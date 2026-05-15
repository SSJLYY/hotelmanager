using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Common
{
    public class JwtTokenRevocationService
    {
        private const string RevokedTokenKeyPrefix = "auth:revoked:";
        private readonly ConcurrentDictionary<string, DateTimeOffset> _memoryStore = new();
        private long _memoryProbeCount;
        private long _redisBypassUntilUtcTicks;

        private readonly RedisHelper _redisHelper;
        private readonly ILogger<JwtTokenRevocationService> _logger;
        private readonly bool _useRedis;
        private readonly TimeSpan _fallbackTtl = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _redisOperationTimeout;
        private readonly TimeSpan _redisFailureCooldown;


        public JwtTokenRevocationService(
            IConfiguration configuration,
            RedisHelper redisHelper,
            ILogger<JwtTokenRevocationService> logger)
        {
            _redisHelper = redisHelper;
            _logger = logger;
            _useRedis = ResolveRedisEnabled(configuration);

            var redisSection = configuration.GetSection("Redis");
            var operationTimeoutMs = redisSection.GetValue<int?>("OperationTimeoutMs") ?? 1200;
            var failureCooldownSeconds = redisSection.GetValue<int?>("FailureCooldownSeconds") ?? 30;

            _redisOperationTimeout = TimeSpan.FromMilliseconds(Math.Clamp(operationTimeoutMs, 200, 5000));
            _redisFailureCooldown = TimeSpan.FromSeconds(Math.Clamp(failureCooldownSeconds, 5, 300));
        }

        public async Task RevokeTokenAsync(string token)
        {
            var normalizedToken = NormalizeToken(token);
            if (string.IsNullOrWhiteSpace(normalizedToken))
            {
                return;
            }

            var key = BuildRevokedTokenKey(normalizedToken);
            var ttl = CalculateRevokedTtl(normalizedToken);

            if (CanUseRedis())
            {
                try
                {
                    var db = _redisHelper.GetDatabase();
                    await ExecuteRedisWithTimeoutAsync(
                        () => db.StringSetAsync(key, "1", ttl),
                        "revoke-token");
                    ClearRedisBypass();
                    return;
                }
                catch (Exception ex) when (IsRedisUnavailableException(ex))
                {
                    MarkRedisUnavailable(ex, "revoke-token");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Redis token revoke failed, fallback to memory store.");
                }
            }

            var memoryExpiresAt = DateTimeOffset.UtcNow.Add(ttl);
            _memoryStore.AddOrUpdate(key, _ => memoryExpiresAt, (_, _) => memoryExpiresAt);
        }

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            var normalizedToken = NormalizeToken(token);
            if (string.IsNullOrWhiteSpace(normalizedToken))
            {
                return false;
            }

            var key = BuildRevokedTokenKey(normalizedToken);

            if (CanUseRedis())
            {
                try
                {
                    var db = _redisHelper.GetDatabase();
                    var isRevoked = await ExecuteRedisWithTimeoutAsync(
                        () => db.KeyExistsAsync(key),
                        "revoke-check");
                    ClearRedisBypass();
                    return isRevoked;
                }
                catch (Exception ex) when (IsRedisUnavailableException(ex))
                {
                    MarkRedisUnavailable(ex, "revoke-check");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Redis token revoke-check failed, fallback to memory store.");
                }
            }

            PruneMemoryStoreIfNeeded();
            if (!_memoryStore.TryGetValue(key, out var expiresAt))
            {
                return false;
            }

            if (expiresAt <= DateTimeOffset.UtcNow)
            {
                _memoryStore.TryRemove(key, out _);
                return false;
            }

            return true;
        }

        public static bool TryGetBearerToken(string authorizationHeader, out string token)
        {
            token = string.Empty;
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return false;
            }

            token = NormalizeToken(authorizationHeader);
            return !string.IsNullOrWhiteSpace(token);
        }

        private bool CanUseRedis()
        {
            if (!_useRedis)
            {
                return false;
            }

            var bypassUntilTicks = Interlocked.Read(ref _redisBypassUntilUtcTicks);
            if (bypassUntilTicks <= 0)
            {
                return true;
            }

            return DateTime.UtcNow.Ticks >= bypassUntilTicks;
        }

        private void MarkRedisUnavailable(Exception ex, string operation)
        {
            var bypassUntil = DateTime.UtcNow.Add(_redisFailureCooldown).Ticks;
            Interlocked.Exchange(ref _redisBypassUntilUtcTicks, bypassUntil);

            _logger.LogWarning(
                ex,
                "Redis {Operation} failed, bypass Redis for {CooldownSeconds}s and fallback to memory store.",
                operation,
                _redisFailureCooldown.TotalSeconds);
        }

        private void ClearRedisBypass()
        {
            Interlocked.Exchange(ref _redisBypassUntilUtcTicks, 0);
        }

        private static bool IsRedisUnavailableException(Exception ex)
        {
            return ex is TimeoutException || ex is RedisException;
        }

        private async Task ExecuteRedisWithTimeoutAsync(Func<Task> redisOperation, string operation)
        {
            var redisTask = redisOperation();
            if (await Task.WhenAny(redisTask, Task.Delay(_redisOperationTimeout)) == redisTask)
            {
                await redisTask;
                return;
            }

            ObserveTaskFailure(redisTask);
            throw new TimeoutException($"Redis operation '{operation}' timed out after {_redisOperationTimeout.TotalMilliseconds}ms.");
        }

        private async Task<T> ExecuteRedisWithTimeoutAsync<T>(Func<Task<T>> redisOperation, string operation)
        {
            var redisTask = redisOperation();
            if (await Task.WhenAny(redisTask, Task.Delay(_redisOperationTimeout)) == redisTask)
            {
                return await redisTask;
            }

            ObserveTaskFailure(redisTask);
            throw new TimeoutException($"Redis operation '{operation}' timed out after {_redisOperationTimeout.TotalMilliseconds}ms.");
        }

        private static void ObserveTaskFailure(Task task)
        {
            task.ContinueWith(
                t =>
                {
                    var _ = t.Exception;
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        private static string BuildRevokedTokenKey(string token)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return $"{RevokedTokenKeyPrefix}{Convert.ToHexString(hash).ToLowerInvariant()}";
        }

        private static string NormalizeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return string.Empty;
            }

            var normalized = token.Trim();
            if (normalized.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(7).Trim();
            }

            return normalized;
        }

        private DateTimeOffset? GetTokenExpiresAtUtc(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                if (jwtToken.ValidTo == DateTime.MinValue)
                {
                    _logger.LogWarning("Failed to parse JWT token expiration");
                    return null;
                }

                return new DateTimeOffset(DateTime.SpecifyKind(jwtToken.ValidTo, DateTimeKind.Utc));
            }
            catch
            {
                _logger.LogWarning("Failed to parse JWT token expiration");
                return null;
            }
        }

        private TimeSpan CalculateRevokedTtl(string token)
        {
            var now = DateTimeOffset.UtcNow;
            var expiresAt = GetTokenExpiresAtUtc(token) ?? now.Add(_fallbackTtl);
            var remaining = expiresAt - now;

            if (remaining <= TimeSpan.Zero)
            {
                return TimeSpan.FromMinutes(1);
            }

            var ttlMinutes = Math.Ceiling(remaining.TotalMinutes);
            return TimeSpan.FromMinutes(ttlMinutes);
        }

        private void PruneMemoryStoreIfNeeded()
        {
            if (Interlocked.Increment(ref _memoryProbeCount) % 200 != 0)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            foreach (var kvp in _memoryStore.ToArray())
            {
                if (kvp.Value <= now)
                {
                    _memoryStore.TryRemove(kvp.Key, out _);
                }
            }
        }

        private static bool ResolveRedisEnabled(IConfiguration configuration)
        {
            var redisSection = configuration.GetSection("Redis");
            var enable = redisSection.GetValue<bool?>("Enable")
                ?? redisSection.GetValue<bool?>("Enabled");
            if (enable.HasValue)
            {
                return enable.Value;
            }

            return redisSection.GetValue<bool>("Enabled");
        }
    }
}