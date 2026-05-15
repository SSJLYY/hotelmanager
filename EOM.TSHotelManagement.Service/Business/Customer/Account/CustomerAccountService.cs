using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.CodeLib;
using jvncorelib.EntityLib;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Transactions;

namespace EOM.TSHotelManagement.Service
{
    public partial class CustomerAccountService : ICustomerAccountService
    {
        /// <summary>
        /// 客户账号
        /// </summary>
        private readonly GenericRepository<CustomerAccount> customerAccountRepository;

        /// <summary>
        /// 客户
        /// </summary>
        private readonly GenericRepository<Customer> customerRepository;

        /// <summary>
        /// 角色仓储（用于客户组角色）
        /// </summary>
        private readonly GenericRepository<Role> roleRepository;

        /// <summary>
        /// 用户-角色映射仓储（用于绑定客户至客户组）
        /// </summary>
        private readonly GenericRepository<UserRole> userRoleRepository;

        /// <summary>
        /// 数据保护工具
        /// </summary>
        private readonly DataProtectionHelper dataProtector;

        /// <summary>
        /// JWT加密
        /// </summary>
        private readonly JWTHelper jWTHelper;

        /// <summary>
        /// 2FA服务
        /// </summary>
        private readonly ITwoFactorAuthService twoFactorAuthService;

        /// <summary>
        /// 邮件助手
        /// </summary>
        private readonly MailHelper mailHelper;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<CustomerAccountService> logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerAccountRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="dataProtector"></param>
        /// <param name="jWTHelper"></param>
        /// <param name="httpContextAccessor"></param>
        public CustomerAccountService(GenericRepository<CustomerAccount> customerAccountRepository, GenericRepository<Customer> customerRepository, GenericRepository<Role> roleRepository, GenericRepository<UserRole> userRoleRepository, DataProtectionHelper dataProtector, JWTHelper jWTHelper, ITwoFactorAuthService twoFactorAuthService, MailHelper mailHelper, IHttpContextAccessor httpContextAccessor, ILogger<CustomerAccountService> logger)
        {
            this.customerAccountRepository = customerAccountRepository;
            this.customerRepository = customerRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.dataProtector = dataProtector;
            this.jWTHelper = jWTHelper;
            this.twoFactorAuthService = twoFactorAuthService;
            this.mailHelper = mailHelper;
            _httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="readCustomerAccountInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadCustomerAccountOutputDto> Login(ReadCustomerAccountInputDto readCustomerAccountInputDto)
        {
            if (readCustomerAccountInputDto.Account.IsNullOrEmpty() || readCustomerAccountInputDto.Password.IsNullOrEmpty())
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.BadRequest, Message = LocalizationHelper.GetLocalizedString("Account or Password cannot be empty", "账号或密码不能为空"), Data = new ReadCustomerAccountOutputDto() };

            var customerAccount = customerAccountRepository.AsQueryable().Single(x => x.Account == readCustomerAccountInputDto.Account);
            if (customerAccount == null)
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.NotFound, Message = LocalizationHelper.GetLocalizedString("Account not found", "账号不存在"), Data = new ReadCustomerAccountOutputDto() };

            if (!dataProtector.CompareCustomerData(customerAccount.Password, readCustomerAccountInputDto.Password))
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.Unauthorized, Message = LocalizationHelper.GetLocalizedString("Invalid account or password", "账号或密码错误"), Data = new ReadCustomerAccountOutputDto() };

            var usedRecoveryCode = false;
            if (twoFactorAuthService.RequiresTwoFactor(TwoFactorUserType.Customer, customerAccount.Id))
            {
                if (string.IsNullOrWhiteSpace(readCustomerAccountInputDto.TwoFactorCode))
                {
                    return new SingleOutputDto<ReadCustomerAccountOutputDto>()
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("2FA code is required", "需要输入2FA验证码"),
                        Data = new ReadCustomerAccountOutputDto
                        {
                            Account = customerAccount.Account,
                            Name = customerAccount.Name,
                            RequiresTwoFactor = true
                        }
                    };
                }

                var passed = twoFactorAuthService.VerifyLoginCode(TwoFactorUserType.Customer, customerAccount.Id, readCustomerAccountInputDto.TwoFactorCode, out usedRecoveryCode);
                if (!passed)
                {
                    return new SingleOutputDto<ReadCustomerAccountOutputDto>()
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Invalid 2FA code", "2FA验证码错误"),
                        Data = new ReadCustomerAccountOutputDto
                        {
                            Account = customerAccount.Account,
                            Name = customerAccount.Name,
                            RequiresTwoFactor = true
                        }
                    };
                }
            }

            var copyCustomerAccount = customerAccount;

            var context = _httpContextAccessor.HttpContext;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            customerAccount.LastLoginIp = ipAddress;
            customerAccount.LastLoginTime = DateTime.Now;
            customerAccountRepository.Update(customerAccount);

            copyCustomerAccount.Password = null;

            if (usedRecoveryCode)
            {
                NotifyRecoveryCodeLoginByEmail(copyCustomerAccount.EmailAddress, copyCustomerAccount.Name, copyCustomerAccount.Account);
            }

            var responseResult = EntityMapper.Map<CustomerAccount, ReadCustomerAccountOutputDto>(copyCustomerAccount);

            responseResult.UserToken = jWTHelper.GenerateJWT(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, customerAccount.Name),
                new Claim(ClaimTypes.SerialNumber, customerAccount.CustomerNumber),
                new Claim("account", customerAccount.Account),
            }), 15); // 15分钟有效期，与员工和管理员保持一致
            responseResult.RefreshToken = jWTHelper.GenerateRefreshToken(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.SerialNumber, customerAccount.CustomerNumber),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }));

            return new SingleOutputDto<ReadCustomerAccountOutputDto>()
            {
                Code = BusinessStatusCode.Success,
                Message = LocalizationHelper.GetLocalizedString("Login successful", "登录成功"),
                Data = responseResult,
            };
        }

        /// <summary>
        /// 备用码登录后邮件通知（客户）
        /// </summary>
        /// <param name="emailAddress">邮箱</param>
        /// <param name="customerName">客户名称</param>
        /// <param name="account">客户账号</param>
        private void NotifyRecoveryCodeLoginByEmail(string? emailAddress, string? customerName, string? account)
        {
            if (string.IsNullOrWhiteSpace(emailAddress) || !MailAddress.TryCreate(emailAddress, out _))
            {
                logger.LogWarning("Recovery-code login alert skipped for customer {Account}: invalid email.", account ?? string.Empty);
                return;
            }

            var recipient = emailAddress;
            var name = customerName ?? "Customer";
            var identity = account ?? string.Empty;
            _ = Task.Run(async () =>
            {
                await SendCustomerRecoveryCodeAlertWithRetryAsync(recipient, name, identity);
            });
        }

        private async Task SendCustomerRecoveryCodeAlertWithRetryAsync(string recipient, string customerName, string account)
        {
            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var template = EmailTemplate.GetTwoFactorRecoveryCodeLoginAlertTemplate(customerName, account, DateTime.Now);
                    var sent = mailHelper.SendMail(new List<string> { recipient }, template.Subject, template.Body);
                    if (sent)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Recovery-code alert send failed for customer {Account}, attempt {Attempt}.", account, attempt);
                }

                if (attempt < maxAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt));
                }
            }

            logger.LogWarning("Recovery-code alert send exhausted retries for customer {Account}.", account);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="readCustomerAccountInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadCustomerAccountOutputDto> Register(ReadCustomerAccountInputDto readCustomerAccountInputDto)
        {

            if (readCustomerAccountInputDto.Account.IsNullOrEmpty() || readCustomerAccountInputDto.Password.IsNullOrEmpty())
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.BadRequest, Message = LocalizationHelper.GetLocalizedString("Account or Password cannot be empty", "账号或密码不能为空"), Data = new ReadCustomerAccountOutputDto() };

            if (readCustomerAccountInputDto.Account.Length < 3 || readCustomerAccountInputDto.Account.Length > 20)
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.BadRequest, Message = LocalizationHelper.GetLocalizedString("Account length must be between 3 and 20 characters", "账号长度必须在3到20个字符之间"), Data = new ReadCustomerAccountOutputDto() };

            if (!AccountRegex.IsMatch(readCustomerAccountInputDto.Account))
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.BadRequest, Message = LocalizationHelper.GetLocalizedString("Account can only contain letters, numbers, and underscores", "账号只能包含字母、数字和下划线"), Data = new ReadCustomerAccountOutputDto() };

            if (!PasswordRegex.IsMatch(readCustomerAccountInputDto.Password))
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.BadRequest, Message = LocalizationHelper.GetLocalizedString("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number", "密码必须至少8个字符，并且包含至少一个大写字母、一个小写字母和一个数字"), Data = new ReadCustomerAccountOutputDto() };

            var customerAccount = customerAccountRepository.AsQueryable().Single(x => x.Account == readCustomerAccountInputDto.Account);

            if (customerAccount != null)
                return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.Conflict, Message = LocalizationHelper.GetLocalizedString("Account already exists", "账号已存在"), Data = new ReadCustomerAccountOutputDto() };

            var password = dataProtector.EncryptCustomerData(readCustomerAccountInputDto.Password);
            var customerNumber = new UniqueCode().GetNewId("TS-");

            using (TransactionScope scope = new TransactionScope())
            {
                customerAccount = new CustomerAccount
                {
                    CustomerNumber = customerNumber,
                    Account = readCustomerAccountInputDto.Account,
                    EmailAddress = string.Empty,
                    Name = readCustomerAccountInputDto.Account,
                    Password = password,
                    Status = 0,
                    LastLoginIp = string.Empty,
                    LastLoginTime = DateTime.Now,
                    DataInsUsr = readCustomerAccountInputDto.Account,
                    DataInsDate = DateTime.Now
                };

                var accountResult = customerAccountRepository.Insert(customerAccount);
                if (!accountResult)
                {
                    return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Account insert failed", "账号插入失败"), Data = new ReadCustomerAccountOutputDto() };
                }

                var customerResult = customerRepository.Insert(new Customer
                {
                    CustomerNumber = customerNumber,
                    Name = string.Empty,
                    Gender = 0,
                    CustomerType = 0,
                    PhoneNumber = string.Empty,
                    Address = string.Empty,
                    DateOfBirth = DateOnly.MinValue,
                    IdCardNumber = string.Empty,
                    IdCardType = 0,
                    IsDelete = 0,
                    DataInsUsr = readCustomerAccountInputDto.Account,
                    DataInsDate = DateTime.Now
                });
                if (!customerResult)
                {
                    return new SingleOutputDto<ReadCustomerAccountOutputDto>() { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Customer insert failed", "客户插入失败"), Data = new ReadCustomerAccountOutputDto() };
                }

                // 将客户加入“客户组”角色，便于与管理员一样进行权限配置
                const string customerRoleNumber = "R-CUSTOMER";

                // 确保客户组角色存在
                if (!roleRepository.AsQueryable().Any(r => r.RoleNumber == customerRoleNumber && r.IsDelete != 1))
                {
                    roleRepository.Insert(new Role
                    {
                        RoleNumber = customerRoleNumber,
                        RoleName = LocalizationHelper.GetLocalizedString("Customer Group", "客户组"),
                        RoleDescription = LocalizationHelper.GetLocalizedString("Unified permission group for customers", "客户统一权限组"),
                        IsDelete = 0,
                        DataInsUsr = readCustomerAccountInputDto.Account,
                        DataInsDate = DateTime.Now
                    });
                }

                // 绑定客户到客户组角色
                if (!userRoleRepository.AsQueryable().Any(ur => ur.UserNumber == customerNumber && ur.RoleNumber == customerRoleNumber))
                {
                    userRoleRepository.Insert(new UserRole
                    {
                        UserNumber = customerNumber,
                        RoleNumber = customerRoleNumber,
                        DataInsUsr = readCustomerAccountInputDto.Account,
                        DataInsDate = DateTime.Now
                    });
                }

                customerAccount.UserToken = jWTHelper.GenerateJWT(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, customerAccount.Name),
                    new Claim(ClaimTypes.SerialNumber, customerNumber),
                    new Claim("account", customerAccount.Account),
                }), 10080); // 7天有效期

                scope.Complete();

                return new SingleOutputDto<ReadCustomerAccountOutputDto>()
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Register successful", "注册成功，稍后将自动登录"),
                    Data = new ReadCustomerAccountOutputDto
                    {
                        Account = customerAccount.Account,
                        EmailAddress = customerAccount.EmailAddress,
                        Name = customerAccount.Name,
                        LastLoginIp = customerAccount.LastLoginIp,
                        LastLoginTime = customerAccount.LastLoginTime,
                        Status = customerAccount.Status,
                        UserToken = customerAccount.UserToken,
                        RequiresTwoFactor = false
                    },
                };
            }
        }

        /// <summary>
        /// 获取客户账号 2FA 状态
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus(string customerSerialNumber)
        {
            return twoFactorAuthService.GetStatus(TwoFactorUserType.Customer, customerSerialNumber);
        }

        /// <summary>
        /// 生成客户账号 2FA 绑定信息
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup(string customerSerialNumber)
        {
            return twoFactorAuthService.GenerateSetup(TwoFactorUserType.Customer, customerSerialNumber);
        }

        /// <summary>
        /// 启用客户账号 2FA
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor(string customerSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.Enable(TwoFactorUserType.Customer, customerSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }

        /// <summary>
        /// 关闭客户账号 2FA
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        public BaseResponse DisableTwoFactor(string customerSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.Disable(TwoFactorUserType.Customer, customerSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }

        /// <summary>
        /// 重置客户账号恢复备用码
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码或恢复备用码输入</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes(string customerSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.RegenerateRecoveryCodes(TwoFactorUserType.Customer, customerSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }

        private readonly Regex AccountRegex = new Regex(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);
        private readonly Regex PasswordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", RegexOptions.Compiled);

    }
}
