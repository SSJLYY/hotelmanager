using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.WebApi
{
    public class IdempotencyKeyMiddleware
    {
        private const string IdempotencyHeaderName = "Idempotency-Key";
        private const string TenantHeaderName = "X-Tenant-Id";
        private const string ReplayHeaderName = "X-Idempotent-Replay";
        private const string InProgressStatus = "IN_PROGRESS";
        private const string CompletedStatus = "COMPLETED";
        private const string DefaultContentType = "application/json; charset=utf-8";

        private static readonly ConcurrentDictionary<string, IdempotencyCacheItem> MemoryStore = new();
        private static long _memoryRequestCount;

        private readonly RequestDelegate _next;
        private readonly ILogger<IdempotencyKeyMiddleware> _logger;
        private readonly RedisHelper _redisHelper;
        private readonly bool _enabled;
        private readonly bool _enforceKey;
        private readonly bool _persistFailureResponse;
        private readonly int _maxKeyLength;
        private readonly TimeSpan _inProgressTtl;
        private readonly TimeSpan _completedTtl;
        private readonly bool _useRedis;

        public IdempotencyKeyMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<IdempotencyKeyMiddleware> logger,
            RedisHelper redisHelper)
        {
            _next = next;
            _logger = logger;
            _redisHelper = redisHelper;

            var section = configuration.GetSection("Idempotency");
            _enabled = section.GetValue<bool?>("Enabled") ?? true;
            _enforceKey = section.GetValue<bool?>("EnforceKey") ?? false;
            _persistFailureResponse = section.GetValue<bool?>("PersistFailureResponse") ?? false;
            _maxKeyLength = Math.Max(16, section.GetValue<int?>("MaxKeyLength") ?? 128);

            var inProgressSeconds = section.GetValue<int?>("InProgressTtlSeconds") ?? 120;
            var completedHours = section.GetValue<int?>("CompletedTtlHours") ?? 24;
            _inProgressTtl = TimeSpan.FromSeconds(Math.Clamp(inProgressSeconds, 30, 600));
            _completedTtl = TimeSpan.FromHours(Math.Clamp(completedHours, 1, 168));

            _useRedis = ResolveRedisEnabled(configuration);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_enabled || !IsWriteMethod(context.Request.Method))
            {
                await _next(context);
                return;
            }

            var idempotencyKey = context.Request.Headers[IdempotencyHeaderName].ToString().Trim();
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                if (_enforceKey)
                {
                    await WriteBusinessErrorAsync(
                        context,
                        StatusCodes.Status428PreconditionRequired,
                        BusinessStatusCode.IdempotencyKeyMissing,
                        LocalizationHelper.GetLocalizedString(
                            "Missing Idempotency-Key header.",
                            "缺少 Idempotency-Key 请求头。"));
                    return;
                }

                _logger.LogWarning("Write request missing Idempotency-Key. Method={Method}, Path={Path}", context.Request.Method, context.Request.Path);
                await _next(context);
                return;
            }

            if (idempotencyKey.Length > _maxKeyLength)
            {
                await WriteBusinessErrorAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    BusinessStatusCode.IdempotencyKeyMissing,
                    LocalizationHelper.GetLocalizedString(
                        $"Idempotency-Key exceeds max length {_maxKeyLength}.",
                        $"Idempotency-Key 长度超过最大限制 {_maxKeyLength}。"));
                return;
            }

            var requestHash = await ComputeRequestHashAsync(context.Request);
            var scopeKey = BuildScopeKey(context, idempotencyKey);

            var acquireResult = await AcquireAsync(scopeKey, requestHash);
            if (acquireResult.Decision == IdempotencyDecision.PayloadConflict)
            {
                await WriteBusinessErrorAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    BusinessStatusCode.IdempotencyKeyPayloadConflict,
                    LocalizationHelper.GetLocalizedString(
                        "Idempotency-Key was reused with a different payload.",
                        "Idempotency-Key 被复用且请求体不一致。"));
                return;
            }

            if (acquireResult.Decision == IdempotencyDecision.InProgress)
            {
                await WriteBusinessErrorAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    BusinessStatusCode.IdempotencyRequestInProgress,
                    LocalizationHelper.GetLocalizedString(
                        "A request with the same Idempotency-Key is still in progress.",
                        "相同 Idempotency-Key 的请求仍在处理中。"));
                return;
            }

            if (acquireResult.Decision == IdempotencyDecision.Replay && acquireResult.Record != null)
            {
                await ReplayResponseAsync(context, acquireResult.Record);
                return;
            }

            await ExecuteAndStoreAsync(context, scopeKey, requestHash);
        }

        private async Task ExecuteAndStoreAsync(HttpContext context, string scopeKey, string requestHash)
        {
            var originalResponseBody = context.Response.Body;

            try
            {
                using var responseBuffer = new MemoryStream();
                context.Response.Body = responseBuffer;

                await _next(context);

                responseBuffer.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseBuffer, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
                responseBuffer.Seek(0, SeekOrigin.Begin);
                await responseBuffer.CopyToAsync(originalResponseBody);

                var shouldPersist = _persistFailureResponse || IsSuccessStatusCode(context.Response.StatusCode);
                if (shouldPersist)
                {
                    var completedRecord = new IdempotencyRecord
                    {
                        Status = CompletedStatus,
                        RequestHash = requestHash,
                        HttpStatus = context.Response.StatusCode,
                        ResponseBody = responseBody,
                        ContentType = context.Response.ContentType,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };

                    await SaveCompletedAsync(scopeKey, completedRecord);
                }
                else
                {
                    await ReleaseAsync(scopeKey);
                }
            }
            catch
            {
                await ReleaseAsync(scopeKey);
                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBody;
            }
        }

        private async Task ReplayResponseAsync(HttpContext context, IdempotencyRecord record)
        {
            context.Response.Headers[ReplayHeaderName] = "true";
            context.Response.StatusCode = record.HttpStatus ?? StatusCodes.Status200OK;
            context.Response.ContentType = string.IsNullOrWhiteSpace(record.ContentType) ? DefaultContentType : record.ContentType;

            if (!string.IsNullOrEmpty(record.ResponseBody))
            {
                await context.Response.WriteAsync(record.ResponseBody);
            }
        }

        private async Task<AcquireResult> AcquireAsync(string scopeKey, string requestHash)
        {
            if (_useRedis)
            {
                try
                {
                    return await AcquireFromRedisAsync(scopeKey, requestHash);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Idempotency acquire failed on Redis, fallback to memory store. Scope={Scope}", scopeKey);
                }
            }

            return AcquireFromMemory(scopeKey, requestHash);
        }

        private async Task SaveCompletedAsync(string scopeKey, IdempotencyRecord record)
        {
            if (_useRedis)
            {
                try
                {
                    await SaveCompletedToRedisAsync(scopeKey, record);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Idempotency save-completed failed on Redis, fallback to memory store. Scope={Scope}", scopeKey);
                }
            }

            SaveCompletedToMemory(scopeKey, record);
        }

        private async Task ReleaseAsync(string scopeKey)
        {
            if (_useRedis)
            {
                try
                {
                    await ReleaseFromRedisAsync(scopeKey);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Idempotency release failed on Redis, fallback to memory store. Scope={Scope}", scopeKey);
                }
            }

            ReleaseFromMemory(scopeKey);
        }

        private async Task<AcquireResult> AcquireFromRedisAsync(string scopeKey, string requestHash)
        {
            var now = DateTimeOffset.UtcNow;
            var inProgressRecord = new IdempotencyRecord
            {
                Status = InProgressStatus,
                RequestHash = requestHash,
                CreatedAt = now,
                UpdatedAt = now
            };

            var db = _redisHelper.GetDatabase();
            var inserted = await db.StringSetAsync(
                scopeKey,
                JsonSerializer.Serialize(inProgressRecord),
                _inProgressTtl,
                when: When.NotExists);

            if (inserted)
            {
                return AcquireResult.Proceed();
            }

            var existingValue = await db.StringGetAsync(scopeKey);
            if (existingValue.IsNullOrEmpty)
            {
                inserted = await db.StringSetAsync(
                    scopeKey,
                    JsonSerializer.Serialize(inProgressRecord),
                    _inProgressTtl,
                    when: When.NotExists);

                return inserted ? AcquireResult.Proceed() : AcquireResult.InProgress();
            }

            var existingRecord = DeserializeRecord(existingValue);
            return ResolveDecision(existingRecord, requestHash);
        }

        private async Task SaveCompletedToRedisAsync(string scopeKey, IdempotencyRecord record)
        {
            var db = _redisHelper.GetDatabase();
            await db.StringSetAsync(scopeKey, JsonSerializer.Serialize(record), _completedTtl);
        }

        private async Task ReleaseFromRedisAsync(string scopeKey)
        {
            var db = _redisHelper.GetDatabase();
            await db.KeyDeleteAsync(scopeKey);
        }

        private AcquireResult AcquireFromMemory(string scopeKey, string requestHash)
        {
            PruneMemoryStoreIfNeeded();

            while (true)
            {
                var now = DateTimeOffset.UtcNow;
                if (!MemoryStore.TryGetValue(scopeKey, out var cacheItem))
                {
                    var inProgressRecord = new IdempotencyRecord
                    {
                        Status = InProgressStatus,
                        RequestHash = requestHash,
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    var inserted = MemoryStore.TryAdd(scopeKey, new IdempotencyCacheItem
                    {
                        Record = inProgressRecord,
                        ExpiresAt = now.Add(_inProgressTtl)
                    });

                    if (inserted)
                    {
                        return AcquireResult.Proceed();
                    }

                    continue;
                }

                if (cacheItem.ExpiresAt <= now)
                {
                    MemoryStore.TryRemove(scopeKey, out _);
                    continue;
                }

                return ResolveDecision(cacheItem.Record, requestHash);
            }
        }

        private void SaveCompletedToMemory(string scopeKey, IdempotencyRecord record)
        {
            var expiresAt = DateTimeOffset.UtcNow.Add(_completedTtl);
            MemoryStore.AddOrUpdate(
                scopeKey,
                _ => new IdempotencyCacheItem
                {
                    Record = record,
                    ExpiresAt = expiresAt
                },
                (_, _) => new IdempotencyCacheItem
                {
                    Record = record,
                    ExpiresAt = expiresAt
                });
        }

        private void ReleaseFromMemory(string scopeKey)
        {
            MemoryStore.TryRemove(scopeKey, out _);
        }

        private static AcquireResult ResolveDecision(IdempotencyRecord record, string requestHash)
        {
            if (record == null)
            {
                return AcquireResult.InProgress();
            }

            if (!string.Equals(record.RequestHash, requestHash, StringComparison.Ordinal))
            {
                return AcquireResult.PayloadConflict();
            }

            if (string.Equals(record.Status, CompletedStatus, StringComparison.OrdinalIgnoreCase))
            {
                return AcquireResult.Replay(record);
            }

            return AcquireResult.InProgress();
        }

        private static async Task<string> ComputeRequestHashAsync(HttpRequest request)
        {
            if (!IsJsonContentType(request.ContentType))
            {
                return ComputeSha256Hex(string.Empty);
            }

            if (request.ContentLength.HasValue && request.ContentLength.Value == 0)
            {
                return ComputeSha256Hex(string.Empty);
            }

            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            var canonicalBody = CanonicalizeBody(body);
            return ComputeSha256Hex(canonicalBody);
        }

        private static string CanonicalizeBody(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return string.Empty;
            }

            try
            {
                using var document = JsonDocument.Parse(body);
                using var buffer = new MemoryStream();
                using (var writer = new Utf8JsonWriter(buffer))
                {
                    WriteCanonicalJson(writer, document.RootElement);
                }

                return Encoding.UTF8.GetString(buffer.ToArray());
            }
            catch
            {
                return body.Trim();
            }
        }

        private static void WriteCanonicalJson(Utf8JsonWriter writer, JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    writer.WriteStartObject();
                    foreach (var property in element.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal))
                    {
                        writer.WritePropertyName(property.Name);
                        WriteCanonicalJson(writer, property.Value);
                    }
                    writer.WriteEndObject();
                    return;
                case JsonValueKind.Array:
                    writer.WriteStartArray();
                    foreach (var item in element.EnumerateArray())
                    {
                        WriteCanonicalJson(writer, item);
                    }
                    writer.WriteEndArray();
                    return;
                default:
                    element.WriteTo(writer);
                    return;
            }
        }

        private static string BuildScopeKey(HttpContext context, string idempotencyKey)
        {
            var tenantId = ResolveTenantId(context);
            var userId = ResolveUserId(context);
            var method = context.Request.Method.ToUpperInvariant();
            var normalizedPath = NormalizePath(context.Request.Path.Value);

            var scope = $"{tenantId}:{userId}:{method}:{normalizedPath}:{idempotencyKey}";
            return $"idem:{ComputeSha256Hex(scope)}";
        }

        private static string ResolveTenantId(HttpContext context)
        {
            var tenantId = context.Request.Headers[TenantHeaderName].ToString();
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                return tenantId.Trim().ToLowerInvariant();
            }

            var tenantClaim = context.User?.FindFirst("tenantId")?.Value
                ?? context.User?.FindFirst("tid")?.Value
                ?? "default";

            return tenantClaim.Trim().ToLowerInvariant();
        }

        private static string ResolveUserId(HttpContext context)
        {
            var userId = context.User?.FindFirst(ClaimTypes.SerialNumber)?.Value
                ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User?.Identity?.Name
                ?? "anonymous";

            return userId.Trim().ToLowerInvariant();
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "/";
            }

            var normalized = path.Trim().ToLowerInvariant();
            if (normalized.Length > 1)
            {
                normalized = normalized.TrimEnd('/');
            }

            return string.IsNullOrWhiteSpace(normalized) ? "/" : normalized;
        }

        private static bool ResolveRedisEnabled(IConfiguration configuration)
        {
            var redisSection = configuration.GetSection("Redis");
            var enable = redisSection.GetValue<bool?>("Enable");
            if (enable.HasValue)
            {
                return enable.Value;
            }

            return redisSection.GetValue<bool>("Enabled");
        }

        private static bool IsWriteMethod(string method)
        {
            return HttpMethods.IsPost(method)
                   || HttpMethods.IsPut(method)
                   || HttpMethods.IsPatch(method);
        }

        private static bool IsSuccessStatusCode(int statusCode)
        {
            return statusCode >= 200 && statusCode < 300;
        }

        private static bool IsJsonContentType(string contentType)
        {
            return !string.IsNullOrWhiteSpace(contentType)
                && contentType.Contains("json", StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeSha256Hex(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private static IdempotencyRecord DeserializeRecord(RedisValue value)
        {
            if (value.IsNullOrEmpty)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<IdempotencyRecord>(value.ToString());
            }
            catch
            {
                return null;
            }
        }

        private static void PruneMemoryStoreIfNeeded()
        {
            if (Interlocked.Increment(ref _memoryRequestCount) % 200 != 0)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            foreach (var kvp in MemoryStore)
            {
                if (kvp.Value.ExpiresAt <= now)
                {
                    MemoryStore.TryRemove(kvp.Key, out _);
                }
            }
        }

        private static async Task WriteBusinessErrorAsync(HttpContext context, int httpStatus, int businessCode, string message)
        {
            context.Response.StatusCode = httpStatus;
            context.Response.ContentType = DefaultContentType;

            var response = new BaseResponse(businessCode, message);
            var payload = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DictionaryKeyPolicy = null
            });

            await context.Response.WriteAsync(payload);
        }

        private sealed class IdempotencyCacheItem
        {
            public IdempotencyRecord Record { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
        }

        private sealed class IdempotencyRecord
        {
            public string Status { get; set; }
            public string RequestHash { get; set; }
            public int? HttpStatus { get; set; }
            public string ResponseBody { get; set; }
            public string ContentType { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
            public DateTimeOffset UpdatedAt { get; set; }
        }

        private enum IdempotencyDecision
        {
            Proceed = 0,
            Replay = 1,
            InProgress = 2,
            PayloadConflict = 3
        }

        private sealed class AcquireResult
        {
            private AcquireResult(IdempotencyDecision decision, IdempotencyRecord record = null)
            {
                Decision = decision;
                Record = record;
            }

            public IdempotencyDecision Decision { get; }
            public IdempotencyRecord Record { get; }

            public static AcquireResult Proceed() => new AcquireResult(IdempotencyDecision.Proceed);
            public static AcquireResult Replay(IdempotencyRecord record) => new AcquireResult(IdempotencyDecision.Replay, record);
            public static AcquireResult InProgress() => new AcquireResult(IdempotencyDecision.InProgress);
            public static AcquireResult PayloadConflict() => new AcquireResult(IdempotencyDecision.PayloadConflict);
        }
    }
}
