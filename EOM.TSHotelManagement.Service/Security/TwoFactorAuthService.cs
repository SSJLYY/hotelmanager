using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using Microsoft.Extensions.Logging;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 2FA（TOTP）统一服务实现
    /// </summary>
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly GenericRepository<TwoFactorAuth> _twoFactorRepository;
        private readonly GenericRepository<TwoFactorRecoveryCode> _recoveryCodeRepository;
        private readonly GenericRepository<Domain.Employee> _employeeRepository;
        private readonly GenericRepository<Administrator> _administratorRepository;
        private readonly GenericRepository<CustomerAccount> _customerAccountRepository;
        private readonly DataProtectionHelper _dataProtectionHelper;
        private readonly TwoFactorHelper _twoFactorHelper;
        private readonly ILogger<TwoFactorAuthService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TwoFactorAuthService(
            GenericRepository<TwoFactorAuth> twoFactorRepository,
            GenericRepository<TwoFactorRecoveryCode> recoveryCodeRepository,
            GenericRepository<Domain.Employee> employeeRepository,
            GenericRepository<Administrator> administratorRepository,
            GenericRepository<CustomerAccount> customerAccountRepository,
            DataProtectionHelper dataProtectionHelper,
            TwoFactorHelper twoFactorHelper,
            ILogger<TwoFactorAuthService> logger)
        {
            _twoFactorRepository = twoFactorRepository;
            _recoveryCodeRepository = recoveryCodeRepository;
            _employeeRepository = employeeRepository;
            _administratorRepository = administratorRepository;
            _customerAccountRepository = customerAccountRepository;
            _dataProtectionHelper = dataProtectionHelper;
            _twoFactorHelper = twoFactorHelper;
            _logger = logger;
        }

        /// <summary>
        /// 判断指定账号是否启用了 2FA
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="userPrimaryKey">账号主键ID</param>
        /// <returns></returns>
        public bool RequiresTwoFactor(TwoFactorUserType userType, int userPrimaryKey)
        {
            var auth = GetByUserPrimaryKey(userType, userPrimaryKey);
            return auth != null
                   && auth.IsDelete != 1
                   && auth.IsEnabled == 1
                   && !string.IsNullOrWhiteSpace(auth.SecretKey);
        }

        /// <summary>
        /// 校验登录 2FA 验证码（支持 TOTP 或恢复备用码）
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="userPrimaryKey">账号主键ID</param>
        /// <param name="code">验证码或恢复备用码</param>
        /// <param name="usedRecoveryCode">是否使用了恢复备用码</param>
        /// <returns></returns>
        public bool VerifyLoginCode(TwoFactorUserType userType, int userPrimaryKey, string? code, out bool usedRecoveryCode)
        {
            usedRecoveryCode = false;

            if (string.IsNullOrWhiteSpace(code))
                return false;

            var auth = GetByUserPrimaryKey(userType, userPrimaryKey);
            if (auth == null || auth.IsDelete == 1 || auth.IsEnabled != 1 || string.IsNullOrWhiteSpace(auth.SecretKey))
                return false;

            if (TryVerifyTotp(auth, code, out var validatedCounter))
            {
                return TryMarkTotpValidated(auth, validatedCounter);
            }

            if (!TryConsumeRecoveryCode(auth.Id, code))
            {
                return false;
            }

            usedRecoveryCode = true;
            return TouchLastVerifiedAt(auth.Id);
        }

        /// <summary>
        /// 获取账号 2FA 状态
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorStatusOutputDto> GetStatus(TwoFactorUserType userType, string serialNumber)
        {
            var resolved = ResolveUser(userType, serialNumber);
            if (resolved == null)
            {
                return new SingleOutputDto<TwoFactorStatusOutputDto>
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = LocalizationHelper.GetLocalizedString("User not found", "用户不存在"),
                    Data = null
                };
            }

            var auth = GetByUserPrimaryKey(userType, resolved.UserPrimaryKey);
            return new SingleOutputDto<TwoFactorStatusOutputDto>
            {
                Code = BusinessStatusCode.Success,
                Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                Data = new TwoFactorStatusOutputDto
                {
                    IsEnabled = auth?.IsEnabled == 1,
                    EnabledAt = auth?.EnabledAt,
                    LastVerifiedAt = auth?.LastVerifiedAt,
                    RemainingRecoveryCodes = auth == null ? 0 : GetRemainingRecoveryCodeCount(auth.Id)
                }
            };
        }

        /// <summary>
        /// 生成账号 2FA 绑定信息（密钥与 otpauth URI）
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorSetupOutputDto> GenerateSetup(TwoFactorUserType userType, string serialNumber)
        {
            try
            {
                var resolved = ResolveUser(userType, serialNumber);
                if (resolved == null)
                {
                    return new SingleOutputDto<TwoFactorSetupOutputDto>
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("User not found", "用户不存在"),
                        Data = null
                    };
                }

                var secret = _twoFactorHelper.GenerateSecretKey();
                var encryptedSecret = _dataProtectionHelper.EncryptTwoFactorData(secret);
                var auth = GetByUserPrimaryKey(userType, resolved.UserPrimaryKey);

                if (auth == null)
                {
                    auth = new TwoFactorAuth
                    {
                        IsEnabled = 0,
                        SecretKey = encryptedSecret,
                        EnabledAt = null,
                        LastVerifiedAt = null,
                        LastValidatedCounter = null
                    };
                    AttachUserForeignKey(auth, userType, resolved.UserPrimaryKey);
                    _twoFactorRepository.Insert(auth);
                    auth = GetByUserPrimaryKey(userType, resolved.UserPrimaryKey);
                }
                else
                {
                    auth.SecretKey = encryptedSecret;
                    auth.IsEnabled = 0;
                    auth.EnabledAt = null;
                    auth.LastVerifiedAt = null;
                    auth.LastValidatedCounter = null;
                    _twoFactorRepository.Update(auth);
                }

                if (auth != null)
                {
                    ClearRecoveryCodes(auth.Id);
                }

                return new SingleOutputDto<TwoFactorSetupOutputDto>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("2FA setup created", "2FA绑定信息已生成"),
                    Data = new TwoFactorSetupOutputDto
                    {
                        IsEnabled = false,
                        AccountName = resolved.AccountName,
                        OtpAuthUri = _twoFactorHelper.BuildOtpAuthUri(resolved.AccountName, secret),
                        CodeDigits = _twoFactorHelper.GetCodeDigits(),
                        TimeStepSeconds = _twoFactorHelper.GetTimeStepSeconds()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GenerateSetup failed for {UserType}-{SerialNumber}", userType, serialNumber);
                return new SingleOutputDto<TwoFactorSetupOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("2FA setup failed", "2FA绑定信息生成失败"),
                    Data = null
                };
            }
        }

        /// <summary>
        /// 启用账号 2FA
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <param name="verificationCode">验证码</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> Enable(TwoFactorUserType userType, string serialNumber, string verificationCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(verificationCode))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Verification code is required", "验证码不能为空"),
                        Data = null
                    };
                }

                var resolved = ResolveUser(userType, serialNumber);
                if (resolved == null)
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("User not found", "用户不存在"),
                        Data = null
                    };
                }

                var auth = GetByUserPrimaryKey(userType, resolved.UserPrimaryKey);
                if (auth == null || string.IsNullOrWhiteSpace(auth.SecretKey))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Please generate 2FA setup first", "请先生成2FA绑定信息"),
                        Data = null
                    };
                }

                if (!TryVerifyTotp(auth, verificationCode, out var validatedCounter))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Invalid 2FA code", "2FA验证码错误"),
                        Data = null
                    };
                }

                if (!TryMarkTotpValidated(auth, validatedCounter))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("2FA code has already been used", "该2FA验证码已被使用，请等待下一个验证码。"),
                        Data = null
                    };
                }

                auth.IsEnabled = 1;
                auth.EnabledAt = DateTime.Now;
                if (!_twoFactorRepository.Update(auth))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = LocalizationHelper.GetLocalizedString("Enable 2FA failed", "鍚敤2FA澶辫触"),
                        Data = null
                    };
                }

                // 启用时自动生成一组恢复备用码（仅保存哈希，明文只在本次响应返回）
                var codes = ReplaceRecoveryCodes(auth.Id);

                return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("2FA enabled", "2FA已启用"),
                    Data = new TwoFactorRecoveryCodesOutputDto
                    {
                        RecoveryCodes = codes,
                        RemainingCount = codes.Count
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enable 2FA failed for {UserType}-{SerialNumber}", userType, serialNumber);
                return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("Enable 2FA failed", "启用2FA失败"),
                    Data = null
                };
            }
        }

        /// <summary>
        /// 关闭账号 2FA
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <param name="verificationCode">验证码或恢复备用码</param>
        /// <returns></returns>
        public BaseResponse Disable(TwoFactorUserType userType, string serialNumber, string verificationCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(verificationCode))
                {
                    return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("Verification code is required", "验证码不能为空"));
                }

                var resolved = ResolveUser(userType, serialNumber);
                if (resolved == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("User not found", "用户不存在"));
                }

                var auth = GetByUserPrimaryKey(userType, resolved.UserPrimaryKey);
                if (auth == null || auth.IsEnabled != 1 || string.IsNullOrWhiteSpace(auth.SecretKey))
                {
                    return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("2FA is not enabled", "2FA未启用"));
                }

                if (!VerifyTotpOrRecoveryCode(auth, verificationCode, allowRecoveryCode: true))
                {
                    return new BaseResponse(BusinessStatusCode.Unauthorized, LocalizationHelper.GetLocalizedString("Invalid 2FA code", "2FA验证码错误"));
                }

                auth.IsEnabled = 0;
                auth.SecretKey = null;
                auth.EnabledAt = null;
                auth.LastVerifiedAt = DateTime.Now;
                auth.LastValidatedCounter = null;
                if (!_twoFactorRepository.Update(auth))
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Disable 2FA failed", "鍏抽棴2FA澶辫触"));
                }

                ClearRecoveryCodes(auth.Id);

                return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("2FA disabled", "2FA已关闭"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Disable 2FA failed for {UserType}-{SerialNumber}", userType, serialNumber);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Disable 2FA failed", "关闭2FA失败"));
            }
        }

        /// <summary>
        /// 重置恢复备用码（会使旧备用码全部失效）
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <param name="verificationCode">验证码或恢复备用码</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateRecoveryCodes(TwoFactorUserType userType, string serialNumber, string verificationCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(verificationCode))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Verification code is required", "验证码不能为空"),
                        Data = null
                    };
                }

                var resolved = ResolveUser(userType, serialNumber);
                if (resolved == null)
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("User not found", "用户不存在"),
                        Data = null
                    };
                }

                var auth = GetByUserPrimaryKey(userType, resolved.UserPrimaryKey);
                if (auth == null || auth.IsEnabled != 1 || string.IsNullOrWhiteSpace(auth.SecretKey))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("2FA is not enabled", "2FA未启用"),
                        Data = null
                    };
                }

                if (!VerifyTotpOrRecoveryCode(auth, verificationCode, allowRecoveryCode: true))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Invalid 2FA code", "2FA验证码错误"),
                        Data = null
                    };
                }

                var codes = ReplaceRecoveryCodes(auth.Id);
                if (!TouchLastVerifiedAt(auth.Id))
                {
                    return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = LocalizationHelper.GetLocalizedString("Recovery code regenerate failed", "备用码生成失败"),
                        Data = null
                    };
                }

                return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Recovery codes regenerated", "恢复备用码已重置"),
                    Data = new TwoFactorRecoveryCodesOutputDto
                    {
                        RecoveryCodes = codes,
                        RemainingCount = codes.Count
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegenerateRecoveryCodes failed for {UserType}-{SerialNumber}", userType, serialNumber);
                return new SingleOutputDto<TwoFactorRecoveryCodesOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("Recovery code regenerate failed", "恢复备用码重置失败"),
                    Data = null
                };
            }
        }

        /// <summary>
        /// 按账号主键查询 2FA 记录
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="userPrimaryKey">账号主键ID</param>
        /// <returns></returns>
        private TwoFactorAuth? GetByUserPrimaryKey(TwoFactorUserType userType, int userPrimaryKey)
        {
            return userType switch
            {
                TwoFactorUserType.Employee => _twoFactorRepository.GetFirst(a => a.EmployeePk == userPrimaryKey && a.IsDelete != 1),
                TwoFactorUserType.Administrator => _twoFactorRepository.GetFirst(a => a.AdministratorPk == userPrimaryKey && a.IsDelete != 1),
                TwoFactorUserType.Customer => _twoFactorRepository.GetFirst(a => a.CustomerAccountPk == userPrimaryKey && a.IsDelete != 1),
                _ => null
            };
        }

        /// <summary>
        /// 写入对应账号类型的外键字段
        /// </summary>
        /// <param name="auth">2FA 实体</param>
        /// <param name="userType">账号类型</param>
        /// <param name="userPrimaryKey">账号主键ID</param>
        private static void AttachUserForeignKey(TwoFactorAuth auth, TwoFactorUserType userType, int userPrimaryKey)
        {
            switch (userType)
            {
                case TwoFactorUserType.Employee:
                    auth.EmployeePk = userPrimaryKey;
                    break;
                case TwoFactorUserType.Administrator:
                    auth.AdministratorPk = userPrimaryKey;
                    break;
                case TwoFactorUserType.Customer:
                    auth.CustomerAccountPk = userPrimaryKey;
                    break;
            }
        }

        /// <summary>
        /// 校验 TOTP，失败后可回退校验恢复备用码（并一次性消费）
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="code"></param>
        /// <param name="allowRecoveryCode"></param>
        /// <returns></returns>
        private bool VerifyTotpOrRecoveryCode(TwoFactorAuth auth, string code, bool allowRecoveryCode)
        {
            if (string.IsNullOrWhiteSpace(auth.SecretKey) || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            if (TryVerifyTotp(auth, code, out var validatedCounter))
            {
                return TryMarkTotpValidated(auth, validatedCounter);
            }

            return allowRecoveryCode && TryConsumeRecoveryCode(auth.Id, code);
        }

        /// <summary>
        /// 校验TOTP（不处理重放）
        /// </summary>
        private bool TryVerifyTotp(TwoFactorAuth auth, string code, out long validatedCounter)
        {
            validatedCounter = -1;
            if (string.IsNullOrWhiteSpace(auth.SecretKey) || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var encryptedSecret = auth.SecretKey;
            var secret = _dataProtectionHelper.SafeDecryptTwoFactorData(encryptedSecret);

            // Opportunistic migration: move legacy plaintext secrets to protected storage.
            if (!_dataProtectionHelper.IsTwoFactorDataProtected(encryptedSecret) && !string.IsNullOrWhiteSpace(secret))
            {
                var migratedSecret = _dataProtectionHelper.EncryptTwoFactorData(secret);
                if (!string.Equals(migratedSecret, encryptedSecret, StringComparison.Ordinal))
                {
                    auth.SecretKey = migratedSecret;
                    if (!_twoFactorRepository.Update(auth))
                    {
                        _logger.LogWarning("Failed to migrate legacy plaintext 2FA secret for auth id {AuthId}", auth.Id);
                        auth.SecretKey = encryptedSecret;
                    }
                }
            }

            return _twoFactorHelper.TryVerifyCode(secret, code, out validatedCounter);
        }

        /// <summary>
        /// TOTP防重放：拒绝重复或倒退counter
        /// </summary>
        private bool TryMarkTotpValidated(TwoFactorAuth auth, long validatedCounter)
        {
            var now = DateTime.Now;
            var affected = _twoFactorRepository.Context
                .Updateable<TwoFactorAuth>()
                .SetColumns(a => a.LastValidatedCounter == validatedCounter)
                .SetColumns(a => a.LastVerifiedAt == now)
                .Where(a => a.Id == auth.Id && a.IsDelete != 1)
                .Where(a => a.LastValidatedCounter == null || a.LastValidatedCounter < validatedCounter)
                .ExecuteCommand();

            if (affected <= 0)
            {
                return false;
            }

            auth.LastValidatedCounter = validatedCounter;
            auth.LastVerifiedAt = now;
            return true;
        }

        private bool TouchLastVerifiedAt(int authId)
        {
            var now = DateTime.Now;
            return _twoFactorRepository.Context
                .Updateable<TwoFactorAuth>()
                .SetColumns(a => a.LastVerifiedAt == now)
                .Where(a => a.Id == authId && a.IsDelete != 1)
                .ExecuteCommand() > 0;
        }

        /// <summary>
        /// 获取剩余可用恢复备用码数量
        /// </summary>
        /// <param name="twoFactorAuthId"></param>
        /// <returns></returns>
        private int GetRemainingRecoveryCodeCount(int twoFactorAuthId)
        {
            return _recoveryCodeRepository
                .Count(a => a.TwoFactorAuthPk == twoFactorAuthId && a.IsDelete != 1 && a.IsUsed != 1);
        }

        /// <summary>
        /// 清理指定 2FA 的全部恢复备用码（硬删除）
        /// </summary>
        /// <param name="twoFactorAuthId"></param>
        private void ClearRecoveryCodes(int twoFactorAuthId)
        {
            _recoveryCodeRepository.Delete(a => a.TwoFactorAuthPk == twoFactorAuthId);
        }

        /// <summary>
        /// 重新生成恢复备用码（会清理旧数据）
        /// </summary>
        /// <param name="twoFactorAuthId"></param>
        /// <returns>新备用码明文（仅返回一次）</returns>
        private List<string> ReplaceRecoveryCodes(int twoFactorAuthId)
        {
            ClearRecoveryCodes(twoFactorAuthId);

            var plainCodes = _twoFactorHelper.GenerateRecoveryCodes();
            foreach (var code in plainCodes)
            {
                var salt = _twoFactorHelper.CreateRecoveryCodeSalt();
                var hash = _twoFactorHelper.HashRecoveryCode(code, salt);

                _recoveryCodeRepository.Insert(new TwoFactorRecoveryCode
                {
                    TwoFactorAuthPk = twoFactorAuthId,
                    CodeSalt = salt,
                    CodeHash = hash,
                    IsUsed = 0,
                    UsedAt = null,
                    IsDelete = 0
                });
            }

            return plainCodes;
        }

        /// <summary>
        /// 尝试消费一个恢复备用码（一次性）
        /// </summary>
        /// <param name="twoFactorAuthId"></param>
        /// <param name="candidateCode"></param>
        /// <returns></returns>
        private bool TryConsumeRecoveryCode(int twoFactorAuthId, string candidateCode)
        {
            var candidates = _recoveryCodeRepository
                .GetList(a => a.TwoFactorAuthPk == twoFactorAuthId && a.IsDelete != 1 && a.IsUsed != 1);

            foreach (var item in candidates)
            {
                if (!_twoFactorHelper.VerifyRecoveryCode(candidateCode, item.CodeSalt, item.CodeHash))
                {
                    continue;
                }

                item.IsUsed = 1;
                item.UsedAt = DateTime.Now;
                _recoveryCodeRepository.Update(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 通过业务编号解析账号主键与账号标识
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <returns></returns>
        private UserResolveResult? ResolveUser(TwoFactorUserType userType, string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                return null;

            switch (userType)
            {
                case TwoFactorUserType.Employee:
                    var employee = _employeeRepository.GetFirst(a => a.EmployeeId == serialNumber && a.IsDelete != 1);
                    if (employee == null)
                        return null;
                    return new UserResolveResult(employee.Id, employee.EmployeeId);

                case TwoFactorUserType.Administrator:
                    var admin = _administratorRepository.GetFirst(a => a.Number == serialNumber && a.IsDelete != 1);
                    if (admin == null)
                        return null;
                    return new UserResolveResult(admin.Id, admin.Account ?? admin.Number);

                case TwoFactorUserType.Customer:
                    var customer = _customerAccountRepository.GetFirst(a => a.CustomerNumber == serialNumber && a.IsDelete != 1);
                    if (customer == null)
                        return null;
                    return new UserResolveResult(customer.Id, customer.Account ?? customer.CustomerNumber);

                default:
                    return null;
            }
        }

        /// <summary>
        /// 账号解析结果
        /// </summary>
        private sealed class UserResolveResult
        {
            /// <summary>
            /// 账号主键ID
            /// </summary>
            public int UserPrimaryKey { get; }

            /// <summary>
            /// 账号名称（用于构建 TOTP 标识）
            /// </summary>
            public string AccountName { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="userPrimaryKey"></param>
            /// <param name="accountName"></param>
            public UserResolveResult(int userPrimaryKey, string accountName)
            {
                UserPrimaryKey = userPrimaryKey;
                AccountName = accountName;
            }
        }
    }
}
