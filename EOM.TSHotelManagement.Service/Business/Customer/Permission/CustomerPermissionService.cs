using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// filename OR language.declaration()
    /// CustomerPermissionService.CustomerPermissionService():1
    /// <summary>
    /// 客户组权限分配服务实现
    /// </summary>
    public class CustomerPermissionService : ICustomerPermissionService
    {
        private readonly GenericRepository<Customer> customerRepository;
        private readonly GenericRepository<UserRole> userRoleRepository;
        private readonly GenericRepository<RolePermission> rolePermissionRepository;
        private readonly GenericRepository<Permission> permissionRepository;
        private readonly GenericRepository<Menu> menuRepository;
        private readonly GenericRepository<Role> roleRepository;

        public CustomerPermissionService(
            GenericRepository<Customer> customerRepository,
            GenericRepository<UserRole> userRoleRepository,
            GenericRepository<RolePermission> rolePermissionRepository,
            GenericRepository<Permission> permissionRepository,
            GenericRepository<Menu> menuRepository,
            GenericRepository<Role> roleRepository)
        {
            this.customerRepository = customerRepository;
            this.userRoleRepository = userRoleRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            this.permissionRepository = permissionRepository;
            this.menuRepository = menuRepository;
            this.roleRepository = roleRepository;
        }

        /// filename OR language.declaration()
        /// CustomerPermissionService.AssignUserRoles():1
        public BaseResponse AssignUserRoles(AssignUserRolesInputDto input)
        {
            if (input == null || input.UserNumber.IsNullOrEmpty())
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("UserNumber is required", "缺少用户编码"));
            }

            try
            {
                // 校验客户是否存在且未删除
                var userExists = customerRepository.IsAny(c => c.CustomerNumber == input.UserNumber && c.IsDelete != 1);
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
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString($"Assign roles failed: {ex.Message}", $"分配角色失败：{ex.Message}"));
            }
        }

        /// filename OR language.declaration()
        /// CustomerPermissionService.ReadUserRoles():1
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

        /// filename OR language.declaration()
        /// CustomerPermissionService.ReadUserRolePermissions():1
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

                // 2) 角色 -> 权限编码（RolePermission）
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

        /// filename OR language.declaration()
        /// CustomerPermissionService.AssignUserPermissions():1
        public BaseResponse AssignUserPermissions(AssignUserPermissionsInputDto input)
        {
            if (input == null || input.UserNumber.IsNullOrEmpty())
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("UserNumber is required", "缺少用户编码"));
            }

            try
            {
                // 校验客户是否存在且未删除
                var userExists = customerRepository.IsAny(c => c.CustomerNumber == input.UserNumber && c.IsDelete != 1);
                if (!userExists)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("User not found", "用户不存在"));
                }

                var userRoleNumber = $"R-USER-{input.UserNumber}";

                // 确保专属角色存在
                var existsRole = roleRepository.IsAny(r => r.RoleNumber == userRoleNumber && r.IsDelete != 1);
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
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString($"Assign direct permissions failed: {ex.Message}", $"分配直接权限失败：{ex.Message}"));
            }
        }

        /// filename OR language.declaration()
        /// CustomerPermissionService.ReadUserDirectPermissions():1
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
    }
}
