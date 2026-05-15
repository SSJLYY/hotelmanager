using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 收藏夹服务实现
    /// </summary>
    public class FavoriteCollectionService : IFavoriteCollectionService
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

        private readonly GenericRepository<UserFavoriteCollection> _favoriteCollectionRepository;
        private readonly GenericRepository<Administrator> _administratorRepository;
        private readonly GenericRepository<Domain.Employee> _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FavoriteCollectionService> _logger;

        /// <summary>
        /// 构造收藏夹服务
        /// </summary>
        /// <param name="favoriteCollectionRepository">收藏夹仓储</param>
        /// <param name="administratorRepository">管理员仓储</param>
        /// <param name="employeeRepository">员工仓储</param>
        /// <param name="httpContextAccessor">HTTP 上下文访问器</param>
        /// <param name="logger">日志组件</param>
        public FavoriteCollectionService(
            GenericRepository<UserFavoriteCollection> favoriteCollectionRepository,
            GenericRepository<Administrator> administratorRepository,
            GenericRepository<Domain.Employee> employeeRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<FavoriteCollectionService> logger)
        {
            _favoriteCollectionRepository = favoriteCollectionRepository;
            _administratorRepository = administratorRepository;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// 保存当前登录用户的收藏夹快照
        /// </summary>
        /// <param name="input">收藏夹保存请求</param>
        /// <returns>保存结果</returns>
        public SingleOutputDto<SaveFavoriteCollectionOutputDto> SaveFavoriteCollection(SaveFavoriteCollectionInputDto input)
        {
            input ??= new SaveFavoriteCollectionInputDto();

            try
            {
                var currentUser = ResolveCurrentUser();
                if (currentUser == null)
                {
                    return new SingleOutputDto<SaveFavoriteCollectionOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Unauthorized.", "Unauthorized."),
                        Data = null
                    };
                }

                if (!TryValidateRequestedIdentity(currentUser, input, out var forbiddenResponse))
                {
                    return forbiddenResponse!;
                }

                var normalizedRoutes = NormalizeRoutes(input.FavoriteRoutes);
                var normalizedUpdatedAt = NormalizeUpdatedAt(input.UpdatedAt);
                var normalizedTriggeredBy = NormalizeText(input.TriggeredBy, 32);
                var favoriteRoutesJson = JsonSerializer.Serialize(normalizedRoutes, JsonSerializerOptions);
                var saveResult = TrySaveSnapshot(currentUser, input.RowVersion, favoriteRoutesJson, normalizedRoutes.Count, normalizedUpdatedAt, normalizedTriggeredBy);

                if (saveResult.Outcome == SaveSnapshotOutcome.Conflict)
                {
                    return new SingleOutputDto<SaveFavoriteCollectionOutputDto>
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString(
                            "Data has been modified by another user. Please refresh and retry.",
                            "数据已被其他用户修改，请刷新后重试。"),
                        Data = null
                    };
                }

                if (saveResult.Outcome == SaveSnapshotOutcome.Failed)
                {
                    return new SingleOutputDto<SaveFavoriteCollectionOutputDto>
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = LocalizationHelper.GetLocalizedString("Failed to save favorite collection.", "Failed to save favorite collection."),
                        Data = null
                    };
                }

                return new SingleOutputDto<SaveFavoriteCollectionOutputDto>
                {
                    Message = LocalizationHelper.GetLocalizedString("Favorite collection saved.", "Favorite collection saved."),
                    Data = new SaveFavoriteCollectionOutputDto
                    {
                        Saved = true,
                        RouteCount = normalizedRoutes.Count,
                        UpdatedAt = normalizedUpdatedAt,
                        RowVersion = saveResult.RowVersion
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save favorite collection.");
                return new SingleOutputDto<SaveFavoriteCollectionOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("Failed to save favorite collection.", "Failed to save favorite collection."),
                    Data = null
                };
            }
        }

        /// <summary>
        /// 获取当前登录用户的收藏夹快照
        /// </summary>
        /// <returns>收藏夹读取结果</returns>
        public SingleOutputDto<ReadFavoriteCollectionOutputDto> GetFavoriteCollection()
        {
            try
            {
                var currentUser = ResolveCurrentUser();
                if (currentUser == null)
                {
                    return new SingleOutputDto<ReadFavoriteCollectionOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Unauthorized.", "Unauthorized."),
                        Data = null
                    };
                }

                var collection = _favoriteCollectionRepository.GetFirst(x => x.UserNumber == currentUser.UserNumber);
                if (collection == null)
                {
                    return new SingleOutputDto<ReadFavoriteCollectionOutputDto>
                    {
                        Message = "OK",
                        Data = new ReadFavoriteCollectionOutputDto()
                    };
                }

                return new SingleOutputDto<ReadFavoriteCollectionOutputDto>
                {
                    Message = "OK",
                    Data = new ReadFavoriteCollectionOutputDto
                    {
                        FavoriteRoutes = DeserializeRoutes(collection.FavoriteRoutesJson),
                        UpdatedAt = DateTime.SpecifyKind(collection.UpdatedAt, DateTimeKind.Utc),
                        RowVersion = collection.RowVersion
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get favorite collection.");
                return new SingleOutputDto<ReadFavoriteCollectionOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("Failed to get favorite collection.", "Failed to get favorite collection."),
                    Data = null
                };
            }
        }

        private SaveSnapshotResult TrySaveSnapshot(
            CurrentUserSnapshot currentUser,
            long? rowVersion,
            string favoriteRoutesJson,
            int routeCount,
            DateTime updatedAt,
            string? triggeredBy)
        {
            const int maxInsertRetryCount = 2;

            for (var attempt = 1; attempt <= maxInsertRetryCount; attempt++)
            {
                var existing = _favoriteCollectionRepository.GetFirst(x => x.UserNumber == currentUser.UserNumber);

                if (existing == null)
                {
                    if (rowVersion.HasValue && rowVersion.Value > 0)
                    {
                        _logger.LogWarning(
                            "Favorite collection insert rejected because client provided stale row version {RowVersion} for user {UserNumber}.",
                            rowVersion.Value,
                            currentUser.UserNumber);
                        return new SaveSnapshotResult(SaveSnapshotOutcome.Conflict);
                    }

                    var entity = new UserFavoriteCollection
                    {
                        UserNumber = currentUser.UserNumber,
                        LoginType = currentUser.LoginType,
                        Account = currentUser.Account,
                        FavoriteRoutesJson = favoriteRoutesJson,
                        RouteCount = routeCount,
                        UpdatedAt = updatedAt,
                        TriggeredBy = triggeredBy
                    };

                    try
                    {
                        if (_favoriteCollectionRepository.Insert(entity))
                        {
                            return new SaveSnapshotResult(SaveSnapshotOutcome.Saved, entity.RowVersion);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Insert favorite collection snapshot failed on attempt {Attempt} for user {UserNumber}.",
                            attempt,
                            currentUser.UserNumber);
                    }

                    continue;
                }

                if (!rowVersion.HasValue || rowVersion.Value <= 0)
                {
                    _logger.LogWarning(
                        "Favorite collection update rejected because row version is missing for user {UserNumber}. CurrentRowVersion={CurrentRowVersion}.",
                        currentUser.UserNumber,
                        existing.RowVersion);
                    return new SaveSnapshotResult(SaveSnapshotOutcome.Conflict);
                }

                existing.RowVersion = rowVersion.Value;
                existing.LoginType = currentUser.LoginType;
                existing.Account = currentUser.Account;
                existing.FavoriteRoutesJson = favoriteRoutesJson;
                existing.RouteCount = routeCount;
                existing.UpdatedAt = updatedAt;
                existing.TriggeredBy = triggeredBy;

                if (_favoriteCollectionRepository.Update(existing))
                {
                    return new SaveSnapshotResult(SaveSnapshotOutcome.Saved, existing.RowVersion);
                }

                _logger.LogWarning(
                    "Favorite collection update hit a concurrency conflict for user {UserNumber}. ExpectedRowVersion={ExpectedRowVersion}.",
                    currentUser.UserNumber,
                    rowVersion.Value);
                return new SaveSnapshotResult(SaveSnapshotOutcome.Conflict);
            }

            return new SaveSnapshotResult(SaveSnapshotOutcome.Failed);
        }

        private bool TryValidateRequestedIdentity(
            CurrentUserSnapshot currentUser,
            SaveFavoriteCollectionInputDto input,
            out SingleOutputDto<SaveFavoriteCollectionOutputDto>? forbiddenResponse)
        {
            forbiddenResponse = null;

            var requestAccount = NormalizeText(input.Account, 128);
            if (!string.IsNullOrWhiteSpace(requestAccount) && !IsCurrentAccount(currentUser, requestAccount))
            {
                _logger.LogWarning(
                    "Favorite collection request account mismatch. UserNumber={UserNumber}, RequestAccount={RequestAccount}, ResolvedAccount={ResolvedAccount}.",
                    currentUser.UserNumber,
                    requestAccount,
                    currentUser.Account);

                forbiddenResponse = new SingleOutputDto<SaveFavoriteCollectionOutputDto>
                {
                    Code = BusinessStatusCode.Forbidden,
                    Message = LocalizationHelper.GetLocalizedString(
                        "Requested identity does not match current user.",
                        "请求身份与当前登录用户不一致。"),
                    Data = null
                };

                return false;
            }

            var requestLoginType = NormalizeText(input.LoginType, 32);
            if (!string.IsNullOrWhiteSpace(requestLoginType)
                && !string.Equals(requestLoginType, currentUser.LoginType, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Favorite collection request login type mismatch. UserNumber={UserNumber}, RequestLoginType={RequestLoginType}, ResolvedLoginType={ResolvedLoginType}.",
                    currentUser.UserNumber,
                    requestLoginType,
                    currentUser.LoginType);

                forbiddenResponse = new SingleOutputDto<SaveFavoriteCollectionOutputDto>
                {
                    Code = BusinessStatusCode.Forbidden,
                    Message = LocalizationHelper.GetLocalizedString(
                        "Requested identity does not match current user.",
                        "请求身份与当前登录用户不一致。"),
                    Data = null
                };

                return false;
            }

            return true;
        }

        private CurrentUserSnapshot? ResolveCurrentUser()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var userNumber = principal?.FindFirst(ClaimTypes.SerialNumber)?.Value
                ?? principal?.FindFirst("serialnumber")?.Value
                ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userNumber))
            {
                return null;
            }

            var administrator = _administratorRepository.GetFirst(x => x.Number == userNumber && x.IsDelete != 1);
            if (administrator != null)
            {
                return new CurrentUserSnapshot(userNumber, "admin", administrator.Account);
            }

            var employee = _employeeRepository.GetFirst(x => x.EmployeeId == userNumber && x.IsDelete != 1);
            if (employee != null)
            {
                return new CurrentUserSnapshot(userNumber, "employee", employee.EmployeeId);
            }

            var fallbackAccount = NormalizeText(
                principal?.FindFirst("account")?.Value ?? principal?.Identity?.Name,
                128);
            var loginType = NormalizeText(
                principal?.FindFirst("login_type")?.Value ?? principal?.FindFirst("logintype")?.Value,
                32) ?? "unknown";

            return new CurrentUserSnapshot(userNumber, loginType, fallbackAccount);
        }

        private static bool IsCurrentAccount(CurrentUserSnapshot currentUser, string requestAccount)
        {
            return string.Equals(requestAccount, currentUser.Account, StringComparison.OrdinalIgnoreCase)
                || string.Equals(requestAccount, currentUser.UserNumber, StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> NormalizeRoutes(IEnumerable<string>? routes)
        {
            var result = new List<string>();
            var uniqueRoutes = new HashSet<string>(StringComparer.Ordinal);

            foreach (var route in routes ?? Enumerable.Empty<string>())
            {
                var normalizedRoute = NormalizeText(route, 2048);
                if (string.IsNullOrWhiteSpace(normalizedRoute))
                {
                    continue;
                }

                if (uniqueRoutes.Add(normalizedRoute))
                {
                    result.Add(normalizedRoute);
                }
            }

            return result;
        }

        private static List<string> DeserializeRoutes(string? favoriteRoutesJson)
        {
            if (string.IsNullOrWhiteSpace(favoriteRoutesJson))
            {
                return new List<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(favoriteRoutesJson, JsonSerializerOptions) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static DateTime NormalizeUpdatedAt(DateTime? updatedAt)
        {
            if (!updatedAt.HasValue)
            {
                return DateTime.UtcNow;
            }

            return updatedAt.Value.Kind switch
            {
                DateTimeKind.Utc => updatedAt.Value,
                DateTimeKind.Local => updatedAt.Value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(updatedAt.Value, DateTimeKind.Utc)
            };
        }

        private static string? NormalizeText(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
        }

        private sealed record CurrentUserSnapshot(string UserNumber, string LoginType, string? Account);
        private sealed record SaveSnapshotResult(SaveSnapshotOutcome Outcome, long RowVersion = 0);

        private enum SaveSnapshotOutcome
        {
            Saved,
            Conflict,
            Failed
        }
    }
}
