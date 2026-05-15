using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Role;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 角色服务接口
    /// </summary>
    public class RoleAppService : IRoleAppService
    {
        /// <summary>
        /// 角色仓储
        /// </summary>
        private readonly GenericRepository<Role> roleRepository;

        /// <summary>
        /// 角色-权限 仓储
        /// </summary>
        private readonly GenericRepository<RolePermission> rolePermissionRepository;

        /// <summary>
        /// 用户-角色 仓储
        /// </summary>
        private readonly GenericRepository<UserRole> userRoleRepository;

        /// <summary>
        /// 权限 仓储
        /// </summary>
        private readonly GenericRepository<Permission> permissionRepository;

        /// <summary>
        /// 菜单 仓储
        /// </summary>
        private readonly GenericRepository<Menu> menuRepository;

        private readonly ILogger<RoleAppService> logger;

        public RoleAppService(
            GenericRepository<Role> roleRepository,
            GenericRepository<RolePermission> rolePermissionRepository,
            GenericRepository<UserRole> userRoleRepository,
            GenericRepository<Permission> permissionRepository,
            GenericRepository<Menu> menuRepository,
            ILogger<RoleAppService> logger)
        {
            this.roleRepository = roleRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            this.userRoleRepository = userRoleRepository;
            this.permissionRepository = permissionRepository;
            this.menuRepository = menuRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="deleteRoleInputDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse DeleteRole(DeleteRoleInputDto deleteRoleInputDto)
        {
            try
            {
                if (deleteRoleInputDto?.DelIds == null || !deleteRoleInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteRoleInputDto);
                var roles = roleRepository.GetList(a => delIds.Contains(a.Id));

                if (!roles.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Role Information Not Found", "角色信息未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteRoleInputDto, roles, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 如果角色组存在关联的权限映射或用户绑定，则不允许删除
                var roleNumbers = roles.Select(r => r.RoleNumber).ToList();
                var hasRolePermissions = rolePermissionRepository.IsAny(rp => roleNumbers.Contains(rp.RoleNumber));
                var hasUserRoles = userRoleRepository.IsAny(ur => roleNumbers.Contains(ur.RoleNumber));
                if (hasRolePermissions || hasUserRoles)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Cannot delete role with existing permissions or user bindings", "无法删除存在权限或用户绑定的角色")
                    };
                }

                var result = roleRepository.SoftDeleteRange(roles);

                if (!result)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = LocalizationHelper.GetLocalizedString("Delete failure", "删除失败")
                    };
                }

                return new BaseResponse
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Delete successful", "删除成功")
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="readRoleInputDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ListOutputDto<ReadRoleOutputDto> SelectRoleList(ReadRoleInputDto readRoleInputDto)
        {
            var where = SqlFilterBuilder.BuildExpression<Role, ReadRoleInputDto>(readRoleInputDto);

            var roles = new List<Role>();

            var count = 0;

            if (!readRoleInputDto.IgnorePaging)
            {
                roles = roleRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readRoleInputDto.Page, readRoleInputDto.PageSize, ref count);
            }
            else
            {
                roles = roleRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = roles.Count;
            }

            var roleList = EntityMapper.MapList<Role, ReadRoleOutputDto>(roles);

            return new ListOutputDto<ReadRoleOutputDto>
            {
                Data = new PagedData<ReadRoleOutputDto>
                {
                    Items = roleList,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="createRoleInputDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse InsertRole(CreateRoleInputDto createRoleInputDto)
        {
            try
            {
                var entity = EntityMapper.Map<CreateRoleInputDto, Role>(createRoleInputDto);
                roleRepository.Insert(entity);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating role: {Message}", ex.Message);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Create Role Error", "创建角色失败"));
            }
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="updateRoleInputDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse UpdateRole(UpdateRoleInputDto updateRoleInputDto)
        {
            try
            {
                var entity = EntityMapper.Map<UpdateRoleInputDto, Role>(updateRoleInputDto);
                var result = roleRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating role: {Message}", ex.Message);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Update Role Error", "更新角色失败"));
            }
        }

        /// <summary>
        /// 为角色授予权限（全量覆盖式）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BaseResponse GrantRolePermissions(GrantRolePermissionsInputDto input)
        {
            if (input == null || input.RoleNumber.IsNullOrEmpty())
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("RoleNumber is required", "缺少角色编码"));
            }

            try
            {
                // 校验角色是否存在且未删除
                var roleExists = roleRepository.IsAny(x => x.RoleNumber == input.RoleNumber && x.IsDelete != 1);
                if (!roleExists)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("Role not found", "角色不存在"));
                }

                // 硬删除该角色现有有效授权映射（菜单 + 权限）
                var existing = rolePermissionRepository.AsQueryable()
                    .Where(x => x.RoleNumber == input.RoleNumber)
                    .ToList();
                if (existing.Count > 0)
                {
                    rolePermissionRepository.Delete(existing);
                }

                // 过滤去重并忽略空白权限码
                var distinctPerms = (input.PermissionNumbers ?? new List<string>())
                    .Where(p => !p.IsNullOrEmpty())
                    .Select(p => p.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // 过滤去重并忽略非法菜单主键
                var distinctMenuIds = (input.MenuIds ?? new List<int>())
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                // 仅保留系统中真实存在的权限编码和菜单主键
                var validPerms = distinctPerms.Count == 0
                    ? new List<string>()
                    : permissionRepository.AsQueryable()
                        .Where(p => distinctPerms.Contains(p.PermissionNumber))
                        .Select(p => p.PermissionNumber)
                        .ToList()
                        .Where(p => !p.IsNullOrEmpty())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                var validMenuIds = distinctMenuIds.Count == 0
                    ? new List<int>()
                    : menuRepository.AsQueryable()
                        .Where(m => m.IsDelete != 1 && distinctMenuIds.Contains(m.Id))
                        .Select(m => m.Id)
                        .ToList()
                        .Distinct()
                        .ToList();

                // 写入角色-权限映射
                foreach (var perm in validPerms)
                {
                    var entity = new RolePermission
                    {
                        RoleNumber = input.RoleNumber,
                        PermissionNumber = perm,
                        MenuId = null
                    };
                    rolePermissionRepository.Insert(entity);
                }

                // 写入角色-菜单映射
                foreach (var menuId in validMenuIds)
                {
                    var entity = new RolePermission
                    {
                        RoleNumber = input.RoleNumber,
                        MenuId = menuId,
                        PermissionNumber = null
                    };
                    rolePermissionRepository.Insert(entity);
                }

                return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Grant permissions success", "授予权限成功"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error granting permissions to role {RoleNumber}: {Message}", input.RoleNumber, ex.Message);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString($"Grant permissions failed: {ex.Message}", $"授予权限失败：{ex.Message}"));
            }
        }

        /// <summary>
        /// 读取指定角色已授予的权限编码集合
        /// </summary>
        public ListOutputDto<string> ReadRolePermissions(string roleNumber)
        {
            if (roleNumber.IsNullOrEmpty())
            {
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("RoleNumber is required", "缺少角色编码"),
                    Data = new PagedData<string> { Items = new List<string>(), TotalCount = 0 }
                };
            }

            try
            {
                var list = rolePermissionRepository.AsQueryable()
                    .Where(rp => rp.RoleNumber == roleNumber
                              && rp.PermissionNumber != null
                              && rp.PermissionNumber != "")
                    .Select(rp => rp.PermissionNumber)
                    .ToList() ?? new List<string>();

                list = list.Where(x => !x.IsNullOrEmpty())
                           .Distinct(StringComparer.OrdinalIgnoreCase)
                           .ToList();

                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                    Data = new PagedData<string> { Items = list, TotalCount = list.Count }
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading permissions for role {RoleNumber}: {Message}", roleNumber, ex.Message);
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Query failed: {ex.Message}", $"查询失败：{ex.Message}"),
                    Data = new PagedData<string> { Items = new List<string>(), TotalCount = 0 }
                };
            }
        }

        /// <summary>
        /// 读取指定角色已授予的菜单与权限（菜单与权限独立）
        /// </summary>
        public SingleOutputDto<ReadRoleGrantOutputDto> ReadRoleGrants(string roleNumber)
        {
            if (roleNumber.IsNullOrEmpty())
            {
                return new SingleOutputDto<ReadRoleGrantOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("RoleNumber is required", "缺少角色编码"),
                    Data = new ReadRoleGrantOutputDto
                    {
                        RoleNumber = string.Empty,
                        PermissionNumbers = new List<string>(),
                        MenuIds = new List<int>()
                    }
                };
            }

            try
            {
                var grants = rolePermissionRepository.AsQueryable()
                    .Where(rp => rp.RoleNumber == roleNumber)
                    .Select(rp => new { rp.PermissionNumber, rp.MenuId })
                    .ToList();

                var permissionNumbers = grants
                    .Where(x => !x.PermissionNumber.IsNullOrEmpty())
                    .Select(x => x.PermissionNumber!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var menuIds = grants
                    .Where(x => x.MenuId.HasValue && x.MenuId.Value > 0)
                    .Select(x => x.MenuId!.Value)
                    .Distinct()
                    .ToList();

                return new SingleOutputDto<ReadRoleGrantOutputDto>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                    Data = new ReadRoleGrantOutputDto
                    {
                        RoleNumber = roleNumber,
                        PermissionNumbers = permissionNumbers,
                        MenuIds = menuIds
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading grants for role {RoleNumber}: {Message}", roleNumber, ex.Message);
                return new SingleOutputDto<ReadRoleGrantOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Query failed: {ex.Message}", $"查询失败：{ex.Message}"),
                    Data = new ReadRoleGrantOutputDto
                    {
                        RoleNumber = roleNumber,
                        PermissionNumbers = new List<string>(),
                        MenuIds = new List<int>()
                    }
                };
            }
        }

        /// <summary>
        /// 读取隶属于指定角色的管理员用户编码集合
        /// </summary>
        public ListOutputDto<string> ReadRoleUsers(string roleNumber)
        {
            if (roleNumber.IsNullOrEmpty())
            {
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("RoleNumber is required", "缺少角色编码"),
                    Data = new PagedData<string> { Items = new List<string>(), TotalCount = 0 }
                };
            }

            try
            {
                var list = userRoleRepository.AsQueryable()
                    .Where(ur => ur.RoleNumber == roleNumber)
                    .Select(ur => ur.UserNumber)
                    .ToList() ?? new List<string>();

                list = list.Where(x => !x.IsNullOrEmpty())
                           .Distinct(StringComparer.OrdinalIgnoreCase)
                           .ToList();

                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                    Data = new PagedData<string> { Items = list, TotalCount = list.Count }
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading users for role {RoleNumber}: {Message}", roleNumber, ex.Message);
                return new ListOutputDto<string>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Query failed: {ex.Message}", $"查询失败：{ex.Message}"),
                    Data = new PagedData<string> { Items = new List<string>(), TotalCount = 0 }
                };
            }
        }

        /// <summary>
        /// 为角色分配管理员（全量覆盖）
        /// </summary>
        public BaseResponse AssignRoleUsers(AssignRoleUsersInputDto input)
        {
            if (input == null || input.RoleNumber.IsNullOrEmpty())
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("RoleNumber is required", "缺少角色编码"));
            }

            try
            {
                // 校验角色是否存在且未删除
                var roleExists = roleRepository.IsAny(r => r.RoleNumber == input.RoleNumber && r.IsDelete != 1);
                if (!roleExists)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("Role not found", "角色不存在"));
                }

                // 软删除该角色当前所有有效的用户绑定
                var existing = userRoleRepository.AsQueryable()
                    .Where(ur => ur.RoleNumber == input.RoleNumber)
                    .Select(ur => new UserRole { RoleNumber = ur.RoleNumber, UserNumber = ur.UserNumber })
                    .ToList();

                foreach (var ur in existing)
                {
                    userRoleRepository.SoftDelete(ur);
                }

                // 过滤去重并忽略空白
                var users = (input.UserNumbers ?? new List<string>())
                    .Where(u => !u.IsNullOrEmpty())
                    .Select(u => u.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // 插入新的绑定
                foreach (var u in users)
                {
                    var entity = new UserRole
                    {
                        RoleNumber = input.RoleNumber,
                        UserNumber = u
                    };
                    userRoleRepository.Insert(entity);
                }

                return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Assign users success", "分配用户成功"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error assigning users to role {RoleNumber}: {Message}", input.RoleNumber, ex.Message);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString($"Assign users failed: {ex.Message}", $"分配用户失败：{ex.Message}"));
            }
        }
    }
}

