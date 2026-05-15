/*
 * MIT License
 *Copyright (c) 2021 易开元(Easy-Open-Meta)
 
 *Permission is hereby granted, free of charge, to any person obtaining a copy
 *of this software and associated documentation files (the "Software"), to deal
 *in the Software without restriction, including without limitation the rights
 *to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *copies of the Software, and to permit persons to whom the Software is
 *furnished to do so, subject to the following conditions:
 
 *The above copyright notice and this permission notice shall be included in all
 *copies or substantial portions of the Software.
 
 *THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *SOFTWARE.
 *
 */
using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 管理员数据访问层
    /// </summary>
    public class AdminService : IAdminService
    {
        /// <summary>
        /// 管理员
        /// </summary>
        private readonly GenericRepository<Administrator> adminRepository;

        /// <summary>
        /// 管理员类型
        /// </summary>
        private readonly GenericRepository<AdministratorType> adminTypeRepository;

        /// <summary>
        /// 用户-角色
        /// </summary>
        private readonly GenericRepository<UserRole> userRoleRepository;

        /// <summary>
        /// 角色-权限
        /// </summary>
        private readonly GenericRepository<RolePermission> rolePermissionRepository;

        /// <summary>
        /// 权限
        /// </summary>
        private readonly GenericRepository<Permission> permissionRepository;

        /// <summary> 
        /// 菜单
        /// </summary>
        private readonly GenericRepository<Menu> menuRepository;

        /// <summary>
        /// 角色
        /// </summary>
        private readonly GenericRepository<Role> roleRepository;

        /// <summary>
        /// 数据保护
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

        private readonly ILogger<AdminService> logger;

        public AdminService(GenericRepository<Administrator> adminRepository, GenericRepository<AdministratorType> adminTypeRepository, GenericRepository<UserRole> userRoleRepository, GenericRepository<RolePermission> rolePermissionRepository, GenericRepository<Permission> permissionRepository, GenericRepository<Menu> menuRepository, GenericRepository<Role> roleRepository, DataProtectionHelper dataProtector, JWTHelper jWTHelper, ITwoFactorAuthService twoFactorAuthService, MailHelper mailHelper, ILogger<AdminService> logger)
        {
            this.adminRepository = adminRepository;
            this.adminTypeRepository = adminTypeRepository;
            this.userRoleRepository = userRoleRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            this.permissionRepository = permissionRepository;
            this.menuRepository = menuRepository;
            this.roleRepository = roleRepository;
            this.dataProtector = dataProtector;
            this.jWTHelper = jWTHelper;
            this.twoFactorAuthService = twoFactorAuthService;
            this.mailHelper = mailHelper;
            this.logger = logger;
        }

        /// <summary>
        /// 后台系统登录
        /// </summary>
        /// <param name="readAdministratorInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadAdministratorOutputDto> Login(ReadAdministratorInputDto readAdministratorInputDto)
        {
            if (readAdministratorInputDto == null)
            {
                throw new ArgumentNullException(nameof(readAdministratorInputDto));
            }

            var existingAdmin = adminRepository.GetFirst(a => a.Account == readAdministratorInputDto.Account);

            if (existingAdmin == null)
            {
                return new SingleOutputDto<ReadAdministratorOutputDto>
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = LocalizationHelper.GetLocalizedString("Administrator does not exist", "管理员不存在"),
                    Data = null
                };
            }

            if (existingAdmin.IsDelete == 1)
            {
                return new SingleOutputDto<ReadAdministratorOutputDto>
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = LocalizationHelper.GetLocalizedString("Administrator does not exist", "管理员不存在"),
                    Data = null
                };
            }


            var originalPwd = dataProtector.SafeDecryptAdministratorData(readAdministratorInputDto.Password);
            var currentPwd = dataProtector.SafeDecryptAdministratorData(existingAdmin.Password);
            var passed = originalPwd == currentPwd;
            if (!passed)
            {
                return new SingleOutputDto<ReadAdministratorOutputDto>
                {
                    Code = BusinessStatusCode.Unauthorized,
                    Message = LocalizationHelper.GetLocalizedString("Invalid account or password", "账号或密码错误"),
                    Data = null
                };
            }

            var usedRecoveryCode = false;
            if (twoFactorAuthService.RequiresTwoFactor(TwoFactorUserType.Administrator, existingAdmin.Id))
            {
                if (string.IsNullOrWhiteSpace(readAdministratorInputDto.TwoFactorCode))
                {
                    return new SingleOutputDto<ReadAdministratorOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("2FA code is required", "需要输入2FA验证码"),
                        Data = new ReadAdministratorOutputDto
                        {
                            Number = existingAdmin.Number,
                            Account = existingAdmin.Account,
                            Name = existingAdmin.Name,
                            RequiresTwoFactor = true
                        }
                    };
                }

                var twoFactorPassed = twoFactorAuthService.VerifyLoginCode(TwoFactorUserType.Administrator, existingAdmin.Id, readAdministratorInputDto.TwoFactorCode, out usedRecoveryCode);
                if (!twoFactorPassed)
                {
                    return new SingleOutputDto<ReadAdministratorOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Invalid 2FA code", "2FA验证码错误"),
                        Data = new ReadAdministratorOutputDto
                        {
                            Number = existingAdmin.Number,
                            Account = existingAdmin.Account,
                            Name = existingAdmin.Name,
                            RequiresTwoFactor = true
                        }
                    };
                }
            }

            existingAdmin.Password = string.Empty;
            
            var source = EntityMapper.Map<Administrator, ReadAdministratorOutputDto>(existingAdmin);
            source.UserToken = jWTHelper.GenerateJWT(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, existingAdmin.Name),
                new Claim(ClaimTypes.SerialNumber, existingAdmin.Number),
                new Claim("is_super_admin", existingAdmin.IsSuperAdmin.ToString())
            }));
            source.RefreshToken = jWTHelper.GenerateRefreshToken(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.SerialNumber, existingAdmin.Number),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }));

            source.RequiresTwoFactor = false;
            source.UsedRecoveryCodeLogin = usedRecoveryCode;
            if (usedRecoveryCode)
            {
                NotifyRecoveryCodeLoginByEmail(existingAdmin.Account, existingAdmin.Name);
            }

            return new SingleOutputDto<ReadAdministratorOutputDto>
            {
                Data = source
            };
        }

        /// <summary>
        /// 备用码登录后邮件通知（管理员）
        /// </summary>
        /// <param name="emailCandidate">候选邮箱（管理员账号）</param>
        /// <param name="displayName">显示名称</param>
        private void NotifyRecoveryCodeLoginByEmail(string? emailCandidate, string? displayName)
        {
            if (string.IsNullOrWhiteSpace(emailCandidate) || !MailAddress.TryCreate(emailCandidate, out _))
            {
                logger.LogWarning("Recovery-code login alert skipped for admin {AdminName}: account is not a valid email.", displayName ?? string.Empty);
                return;
            }

            var recipient = emailCandidate;
            var name = displayName ?? "Administrator";
            _ = Task.Run(async () =>
            {
                await SendRecoveryCodeLoginAlertWithRetryAsync(recipient, name, "admin");
            });
        }

        private async Task SendRecoveryCodeLoginAlertWithRetryAsync(string recipient, string displayName, string accountKind)
        {
            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var template = EmailTemplate.GetTwoFactorRecoveryCodeLoginAlertTemplate(displayName, recipient, DateTime.Now);
                    var sent = mailHelper.SendMail(new List<string> { recipient }, template.Subject, template.Body);
                    if (sent)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Recovery-code alert send failed for {AccountKind} {DisplayName}, attempt {Attempt}.", accountKind, displayName, attempt);
                }

                if (attempt < maxAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt));
                }
            }

            logger.LogWarning("Recovery-code alert send exhausted retries for {AccountKind} {DisplayName}.", accountKind, displayName);
        }

        /// <summary>
        /// 获取所有管理员列表
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadAdministratorOutputDto> GetAllAdminList(ReadAdministratorInputDto readAdministratorInputDto)
        {
            readAdministratorInputDto ??= new ReadAdministratorInputDto();

            var where = SqlFilterBuilder.BuildExpression<Administrator, ReadAdministratorInputDto>(readAdministratorInputDto);
            var query = adminRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            var count = 0;
            List<Administrator> administrators;

            if (!readAdministratorInputDto.IgnorePaging)
            {
                var page = readAdministratorInputDto.Page > 0 ? readAdministratorInputDto.Page : 1;
                var pageSize = readAdministratorInputDto.PageSize > 0 ? readAdministratorInputDto.PageSize : 15;
                administrators = query.ToPageList(page, pageSize, ref count);
            }
            else
            {
                administrators = query.ToList();
                count = administrators.Count;
            }

            var adminTypeMap = adminTypeRepository.AsQueryable()
                .Where(a => a.IsDelete != 1)
                .ToList()
                .GroupBy(a => a.TypeId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.TypeName ?? "");

            List<ReadAdministratorOutputDto> result;
            var useParallelProjection = readAdministratorInputDto.IgnorePaging && administrators.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadAdministratorOutputDto[administrators.Count];
                System.Threading.Tasks.Parallel.For(0, administrators.Count, i =>
                {
                    var source = administrators[i];
                    dtoArray[i] = new ReadAdministratorOutputDto
                    {
                        Id = source.Id,
                        Number = source.Number,
                        Account = source.Account,
                        Password = source.Password,
                        Type = source.Type,
                        Name = source.Name,
                        IsSuperAdmin = source.IsSuperAdmin,
                        IsSuperAdminDescription = source.IsSuperAdmin == 1 ? "是" : "否",
                        TypeName = adminTypeMap.TryGetValue(source.Type, out var typeName) ? typeName : "",
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    };
                });
                result = dtoArray.ToList();
            }
            else
            {
                result = new List<ReadAdministratorOutputDto>(administrators.Count);
                administrators.ForEach(source =>
                {
                    result.Add(new ReadAdministratorOutputDto
                    {
                        Id = source.Id,
                        Number = source.Number,
                        Account = source.Account,
                        Password = source.Password,
                        Type = source.Type,
                        Name = source.Name,
                        IsSuperAdmin = source.IsSuperAdmin,
                        IsSuperAdminDescription = source.IsSuperAdmin == 1 ? "是" : "否",
                        TypeName = adminTypeMap.TryGetValue(source.Type, out var typeName) ? typeName : "",
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    });
                });
            }

            return new ListOutputDto<ReadAdministratorOutputDto>
            {
                Data = new PagedData<ReadAdministratorOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="createAdministratorInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddAdmin(CreateAdministratorInputDto createAdministratorInputDto)
        {
            try
            {
                if (createAdministratorInputDto.IsSuperAdmin == (int)AdminRole.SuperAdmin)
                {
                    var haveSuperAdmin = adminRepository.IsAny(a => a.IsSuperAdmin == 1);
                    if (haveSuperAdmin)
                    {
                        return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Super Administrator already exists", "超级管理员已存在"), Code = BusinessStatusCode.InternalServerError };
                    }
                }
                
                var haveAdmin = adminRepository.IsAny(a => a.Account == createAdministratorInputDto.Account);
                if (haveAdmin)
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Administrator already exists", "管理员已存在"), Code = BusinessStatusCode.InternalServerError };
                }

                createAdministratorInputDto.Password = dataProtector.EncryptAdministratorData(createAdministratorInputDto.Password);
                var admin = EntityMapper.Map<CreateAdministratorInputDto, Administrator>(createAdministratorInputDto);
                adminRepository.Insert(admin);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AddAdmin failed for Account: {Account}", createAdministratorInputDto?.Account);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="updateAdministratorInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdAdmin(UpdateAdministratorInputDto updateAdministratorInputDto)
        {
            try
            {
                updateAdministratorInputDto.Password = dataProtector.EncryptAdministratorData(updateAdministratorInputDto.Password);
                var admin = EntityMapper.Map<UpdateAdministratorInputDto, Administrator>(updateAdministratorInputDto);
                var result = adminRepository.Update(admin);
                if (!result)
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Update Administrator Failed", "更新管理员失败"), Code = BusinessStatusCode.InternalServerError };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdAdmin failed for Id: {Id}", updateAdministratorInputDto?.Id);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="deleteAdministratorInputDto"></param>
        /// <returns></returns>
        public BaseResponse DelAdmin(DeleteAdministratorInputDto deleteAdministratorInputDto)
        {
            try
            {
                if (deleteAdministratorInputDto?.DelIds == null || !deleteAdministratorInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteAdministratorInputDto);
                var administrators = adminRepository.GetList(a => delIds.Contains(a.Id));

                if (!administrators.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Administrator Not Found", "管理员未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteAdministratorInputDto, administrators, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // cannot be delete if is super admin
                var admin = administrators.Any(a => a.IsSuperAdmin == 1);
                if (admin)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = LocalizationHelper.GetLocalizedString("Cannot delete super administrator", "无法删除超级管理员")
                    };
                }

                var result = adminRepository.SoftDeleteRange(administrators);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DelAdmin failed for Ids: {DelIds}", deleteAdministratorInputDto?.DelIds);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 获取所有管理员类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadAdministratorTypeOutputDto> GetAllAdminTypes(ReadAdministratorTypeInputDto readAdministratorTypeInputDto)
        {
            readAdministratorTypeInputDto ??= new ReadAdministratorTypeInputDto();

            var where = SqlFilterBuilder.BuildExpression<AdministratorType, ReadAdministratorTypeInputDto>(readAdministratorTypeInputDto);
            var query = adminTypeRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            var count = 0;
            List<AdministratorType> administratorTypes;

            if (!readAdministratorTypeInputDto.IgnorePaging)
            {
                var page = readAdministratorTypeInputDto.Page > 0 ? readAdministratorTypeInputDto.Page : 1;
                var pageSize = readAdministratorTypeInputDto.PageSize > 0 ? readAdministratorTypeInputDto.PageSize : 15;
                administratorTypes = query.ToPageList(page, pageSize, ref count);
            }
            else
            {
                administratorTypes = query.ToList();
                count = administratorTypes.Count;
            }

            List<ReadAdministratorTypeOutputDto> result;
            var useParallelProjection = readAdministratorTypeInputDto.IgnorePaging && administratorTypes.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadAdministratorTypeOutputDto[administratorTypes.Count];
                System.Threading.Tasks.Parallel.For(0, administratorTypes.Count, i =>
                {
                    var source = administratorTypes[i];
                    dtoArray[i] = new ReadAdministratorTypeOutputDto
                    {
                        Id = source.Id,
                        TypeId = source.TypeId,
                        TypeName = source.TypeName,
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    };
                });
                result = dtoArray.ToList();
            }
            else
            {
                result = new List<ReadAdministratorTypeOutputDto>(administratorTypes.Count);
                administratorTypes.ForEach(source =>
                {
                    result.Add(new ReadAdministratorTypeOutputDto
                    {
                        Id = source.Id,
                        TypeId = source.TypeId,
                        TypeName = source.TypeName,
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    });
                });
            }

            return new ListOutputDto<ReadAdministratorTypeOutputDto>
            {
                Data = new PagedData<ReadAdministratorTypeOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 添加管理员类型
        /// </summary>
        /// <param name="createAdministratorTypeInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddAdminType(CreateAdministratorTypeInputDto createAdministratorTypeInputDto)
        {
            try
            {
                adminTypeRepository.Insert(EntityMapper.Map<CreateAdministratorTypeInputDto, AdministratorType>(createAdministratorTypeInputDto));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AddAdminType failed for TypeId: {TypeId}", createAdministratorTypeInputDto?.TypeId);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 更新管理员类型
        /// </summary>
        /// <param name="updateAdministratorTypeInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdAdminType(UpdateAdministratorTypeInputDto updateAdministratorTypeInputDto)
        {
            try
            {
                adminTypeRepository.Update(EntityMapper.Map<UpdateAdministratorTypeInputDto, AdministratorType>(updateAdministratorTypeInputDto));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdAdminType failed for Id: {Id}", updateAdministratorTypeInputDto?.Id);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除管理员类型
        /// </summary>
        /// <param name="deleteAdministratorTypeInputDto"></param>
        /// <returns></returns>
        public BaseResponse DelAdminType(DeleteAdministratorTypeInputDto deleteAdministratorTypeInputDto)
        {
            try
            {
                if (deleteAdministratorTypeInputDto?.DelIds == null || !deleteAdministratorTypeInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteAdministratorTypeInputDto);
                var administratorTypes = adminTypeRepository.GetList(a => delIds.Contains(a.Id));

                if (!administratorTypes.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Administrator Type Not Found", "管理员类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteAdministratorTypeInputDto, administratorTypes, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // cannot be delete if have administrators
                var haveAdmin = adminRepository.IsAny(a => administratorTypes.Select(a => a.TypeId).Contains(a.Type));
                if (haveAdmin)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = LocalizationHelper.GetLocalizedString("Cannot delete administrator type with existing administrators", "无法删除有管理员的管理员类型")
                    };
                }

                // 批量软删除
                adminTypeRepository.SoftDeleteRange(administratorTypes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DelAdminType failed for Ids: {DelIds}", deleteAdministratorTypeInputDto?.DelIds);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 为用户分配角色（全量覆盖）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BaseResponse AssignUserRoles(AssignUserRolesInputDto input)
        {
            if (input == null || input.UserNumber.IsNullOrEmpty())
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("UserNumber is required", "缺少用户编码"));
            }

            try
            {
                // 校验用户是否存在且未删除
                var userExists = adminRepository.IsAny(a => a.Number == input.UserNumber);
                if (!userExists)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("User not found", "用户不存在"));
                }

                // 硬删除当前用户下所有有效的角色绑定
                var existing = userRoleRepository.AsQueryable()
                    .Where(x => x.UserNumber == input.UserNumber)
                    .Select(x => new UserRole { UserNumber = x.UserNumber, RoleNumber = x.RoleNumber })
                    .ToList();

                foreach (var ur in existing)
                {
                    userRoleRepository.Delete(ur);
                }

                // 过滤、去重、忽略空白
                var roles = (input.RoleNumbers ?? new List<string>())
                    .Where(r => !r.IsNullOrEmpty())
                    .Select(r => r.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // 插入新的角色绑定
                foreach (var role in roles)
                {
                    var entity = new UserRole
                    {
                        UserNumber = input.UserNumber,
                        RoleNumber = role
                    };
                    userRoleRepository.Insert(entity);
                }

                return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Assign roles success", "分配角色成功"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AssignUserRoles failed for UserNumber: {UserNumber}", input?.UserNumber);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString($"Assign roles failed: {ex.Message}", $"分配角色失败：{ex.Message}"));
            }
        }

        /// <summary>
        /// 读取指定用户已分配的角色编码集合
        /// </summary>
        /// <param name="userNumber">用户编码</param>
        /// <returns>角色编码集合（RoleNumber 列表）</returns>
        public ListOutputDto<string> ReadUserRoles(string userNumber)
        {
            if (userNumber.IsNullOrEmpty())
            {
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("UserNumber is required", "缺少用户编码"),
                    Data = new PagedData<string> { Items = new List<string>(), TotalCount = 0 }
                };
            }

            try
            {
                var roleNumbers = userRoleRepository.AsQueryable()
                    .Where(ur => ur.UserNumber == userNumber)
                    .Select(ur => ur.RoleNumber)
                    .ToList();

                roleNumbers = roleNumbers?.Where(r => !r.IsNullOrEmpty())?.Distinct(StringComparer.OrdinalIgnoreCase)?.ToList() ?? new List<string>();

                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                    Data = new PagedData<string>
                    {
                        Items = roleNumbers,
                        TotalCount = roleNumbers.Count
                    }
                };
            }
            catch (Exception ex)
            {
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Query failed: {ex.Message}", $"查询失败：{ex.Message}"),
                    Data = new PagedData<string>
                    {
                        Items = new List<string>(),
                        TotalCount = 0
                    }
                };
            }
        }

        /// <summary>
        /// 读取指定用户的“角色-权限”明细（来自 RolePermission 关联，并联到 Permission 得到权限码与名称）
        /// </summary>
        /// <param name="userNumber">用户编码</param>
        /// <returns>明细列表（包含 RoleNumber、PermissionNumber、PermissionName、MenuKey）</returns>
        public ListOutputDto<UserRolePermissionOutputDto> ReadUserRolePermissions(string userNumber)
        {
            if (userNumber.IsNullOrEmpty())
            {
                return new ListOutputDto<UserRolePermissionOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("UserNumber is required", "缺少用户编码"),
                    Data = new PagedData<UserRolePermissionOutputDto> { Items = new List<UserRolePermissionOutputDto>(), TotalCount = 0 }
                };
            }

            try
            {
                // 1) 用户 -> 角色
                var roleNumbers = userRoleRepository.AsQueryable()
                    .Where(ur => ur.UserNumber == userNumber)
                    .Select(ur => ur.RoleNumber)
                    .ToList();

                roleNumbers = roleNumbers?
                    .Where(r => !r.IsNullOrEmpty())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList() ?? new List<string>();

                if (roleNumbers.Count == 0)
                {
                    return new ListOutputDto<UserRolePermissionOutputDto>
                    {
                        Code = BusinessStatusCode.Success,
                        Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                        Data = new PagedData<UserRolePermissionOutputDto>
                        {
                            Items = new List<UserRolePermissionOutputDto>(),
                            TotalCount = 0
                        }
                    };
                }

                // 2) 角色 -> 权限映射（RolePermission）
                var rolePermList = rolePermissionRepository.AsQueryable()
                    .Where(rp => roleNumbers.Contains(rp.RoleNumber)
                              && rp.PermissionNumber != null
                              && rp.PermissionNumber != "")
                    .Select(rp => new { rp.RoleNumber, rp.PermissionNumber, rp.MenuId })
                    .ToList();

                var permNumbers = rolePermList
                    .Select(x => x.PermissionNumber)
                    .Where(x => !x.IsNullOrEmpty())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // 3) 权限详情（Permission）
                var permInfo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (permNumbers.Count > 0)
                {
                    var info = permissionRepository.AsQueryable()
                        .Where(p => permNumbers.Contains(p.PermissionNumber))
                        .Select(p => new { p.PermissionNumber, p.PermissionName })
                        .ToList();

                    foreach (var p in info)
                    {
                        permInfo[p.PermissionNumber] = p.PermissionName;
                    }
                }

                // 4) 组装输出
                var menuIds = rolePermList
                    .Where(x => x.MenuId.HasValue && x.MenuId.Value > 0)
                    .Select(x => x.MenuId!.Value)
                    .Distinct()
                    .ToList();

                var menuInfo = new Dictionary<int, (string Key, string Name)>();
                if (menuIds.Count > 0)
                {
                    var menus = menuRepository.AsQueryable()
                        .Where(m => m.IsDelete != 1 && menuIds.Contains(m.Id))
                        .Select(m => new { m.Id, m.Key, m.Title })
                        .ToList();

                    foreach (var m in menus)
                    {
                        menuInfo[m.Id] = (m.Key ?? string.Empty, m.Title ?? string.Empty);
                    }
                }

                var resultItems = rolePermList.Select(x =>
                {
                    permInfo.TryGetValue(x.PermissionNumber!, out var permName);
                    var menuId = x.MenuId;
                    var menuKey = string.Empty;
                    var menuName = string.Empty;
                    if (menuId.HasValue && menuInfo.TryGetValue(menuId.Value, out var menuMeta))
                    {
                        menuKey = menuMeta.Key;
                        menuName = menuMeta.Name;
                    }

                    return new UserRolePermissionOutputDto
                    {
                        RoleNumber = x.RoleNumber,
                        PermissionNumber = x.PermissionNumber!,
                        PermissionName = permName,
                        MenuId = menuId,
                        MenuKey = menuKey,
                        MenuName = menuName
                    };
                }).ToList();

                return new ListOutputDto<UserRolePermissionOutputDto>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                    Data = new PagedData<UserRolePermissionOutputDto>
                    {
                        Items = resultItems,
                        TotalCount = resultItems.Count
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ReadUserRolePermissions failed for UserNumber: {UserNumber}", userNumber);
                return new ListOutputDto<UserRolePermissionOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Query failed: {ex.Message}", $"查询失败：{ex.Message}"),
                    Data = new PagedData<UserRolePermissionOutputDto>
                    {
                        Items = new List<UserRolePermissionOutputDto>(),
                        TotalCount = 0
                    }
                };
            }
        }

        /// <summary>
        /// 为指定用户分配“直接权限”（通过专属角色 R-USER-{UserNumber} 写入 RolePermission，全量覆盖）
        /// </summary>
        public BaseResponse AssignUserPermissions(AssignUserPermissionsInputDto input)
        {
            if (input == null || input.UserNumber.IsNullOrEmpty())
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("UserNumber is required", "缺少用户编码"));
            }

            try
            {
                // 校验用户是否存在且未删除
                var userExists = adminRepository.IsAny(a => a.Number == input.UserNumber);
                if (!userExists)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("User not found", "用户不存在"));
                }

                var userRoleNumber = $"R-USER-{input.UserNumber}";

                // 确保专属角色存在
                var existsRole = roleRepository.IsAny(r => r.RoleNumber == userRoleNumber);
                if (!existsRole)
                {
                    var role = new Role
                    {
                        RoleNumber = userRoleNumber,
                        RoleName = $"用户{input.UserNumber}直接权限",
                        RoleDescription = $"用户{input.UserNumber}直接权限",
                        IsDelete = 0
                    };
                    roleRepository.Insert(role);
                }

                // 硬删除当前专属角色下所有有效的角色-权限绑定
                var existing = rolePermissionRepository.AsQueryable()
                    .Where(x => x.RoleNumber == userRoleNumber)
                    .ToList();
                if (existing.Count > 0)
                {
                    rolePermissionRepository.Delete(existing);
                }

                // 过滤、去重、忽略空白
                var perms = (input.PermissionNumbers ?? new List<string>())
                    .Where(p => !p.IsNullOrEmpty())
                    .Select(p => p.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (perms.Count > 0)
                {
                    // 仅保留系统中存在的权限码
                    var validPerms = permissionRepository.AsQueryable()
                        .Where(p => perms.Contains(p.PermissionNumber))
                        .Select(p => p.PermissionNumber)
                        .ToList();

                    foreach (var pnum in validPerms)
                    {
                        var entity = new RolePermission
                        {
                            RoleNumber = userRoleNumber,
                            PermissionNumber = pnum
                        };
                        rolePermissionRepository.Insert(entity);
                    }
                }

                // 确保用户与专属角色绑定存在
                var hasBinding = userRoleRepository.IsAny(ur => ur.UserNumber == input.UserNumber && ur.RoleNumber == userRoleNumber);
                if (!hasBinding)
                {
                    userRoleRepository.Insert(new UserRole
                    {
                        UserNumber = input.UserNumber,
                        RoleNumber = userRoleNumber
                    });
                }

                return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Assign direct permissions success", "分配直接权限成功"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AssignUserPermissions failed for UserNumber: {UserNumber}", input?.UserNumber);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString($"Assign direct permissions failed: {ex.Message}", $"分配直接权限失败：{ex.Message}"));
            }
        }

        /// <summary>
        /// 读取指定用户的“直接权限”（仅来自专属角色 R-USER-{UserNumber} 的权限编码列表）
        /// </summary>
        public ListOutputDto<string> ReadUserDirectPermissions(string userNumber)
        {
            if (userNumber.IsNullOrEmpty())
            {
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("UserNumber is required", "缺少用户编码"),
                    Data = new PagedData<string> { Items = new List<string>(), TotalCount = 0 }
                };
            }

            try
            {
                var roleNumber = $"R-USER-{userNumber}";
                var list = rolePermissionRepository.AsQueryable()
                    .Where(rp => rp.RoleNumber == roleNumber)
                    .Select(rp => rp.PermissionNumber)
                    .ToList();

                list = list?.Where(x => !x.IsNullOrEmpty())?.Distinct(StringComparer.OrdinalIgnoreCase)?.ToList() ?? new List<string>();

                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                    Data = new PagedData<string> { Items = list, TotalCount = list.Count }
                };
            }
            catch (Exception ex)
            {
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Query failed: {ex.Message}", $"查询失败：{ex.Message}"),
                    Data = new PagedData<string> { Items = new List<string>(), TotalCount = 0 }
                };
            }
        }

        /// <summary>
        /// 获取管理员账号 2FA 状态
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus(string adminSerialNumber)
        {
            return twoFactorAuthService.GetStatus(TwoFactorUserType.Administrator, adminSerialNumber);
        }

        /// <summary>
        /// 生成管理员账号 2FA 绑定信息
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup(string adminSerialNumber)
        {
            return twoFactorAuthService.GenerateSetup(TwoFactorUserType.Administrator, adminSerialNumber);
        }

        /// <summary>
        /// 启用管理员账号 2FA
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor(string adminSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.Enable(TwoFactorUserType.Administrator, adminSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }

        /// <summary>
        /// 关闭管理员账号 2FA
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        public BaseResponse DisableTwoFactor(string adminSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.Disable(TwoFactorUserType.Administrator, adminSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }

        /// <summary>
        /// 重置管理员账号恢复备用码
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码或恢复备用码输入</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes(string adminSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.RegenerateRecoveryCodes(TwoFactorUserType.Administrator, adminSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }
    }
}
