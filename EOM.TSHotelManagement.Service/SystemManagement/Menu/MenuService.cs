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

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 菜单接口实现类
    /// </summary>
    public class MenuService : IMenuService
    {
        /// <summary>
        /// 菜单
        /// </summary>
        private readonly GenericRepository<Menu> menuRepository;

        /// <summary>
        /// JWT助手
        /// </summary>
        private readonly JWTHelper jWTHelper;
        private readonly GenericRepository<Permission> permissionRepository;
        private readonly GenericRepository<RolePermission> rolePermissionRepository;
        private readonly GenericRepository<UserRole> userRoleRepository;
        private readonly GenericRepository<Administrator> administratorRepository;
        private readonly ILogger<MenuService> logger;

        public MenuService(GenericRepository<Menu> menuRepository, JWTHelper jWTHelper, GenericRepository<Permission> permissionRepository, GenericRepository<RolePermission> rolePermissionRepository, GenericRepository<UserRole> userRoleRepository, GenericRepository<Administrator> administratorRepository, ILogger<MenuService> logger)
        {
            this.menuRepository = menuRepository;
            this.jWTHelper = jWTHelper;
            this.permissionRepository = permissionRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            this.userRoleRepository = userRoleRepository;
            this.administratorRepository = administratorRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 构建菜单树
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<MenuDto> BuildMenuAll(BaseInputDto baseInputDto)
        {
            // 1) 读取所有未删除菜单
            List<Menu> allMenus = menuRepository.GetList(a => a.IsDelete != 1).OrderBy(a => a.Id).ToList();

            // 默认：无权限返回空树
            List<Menu> filteredMenus = new();
            Dictionary<string, List<string>> menuPermMap = new();

            try
            {
                var token = baseInputDto?.UserToken;
                var userNumber = string.Empty;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    try
                    {
                        userNumber = jWTHelper.GetSerialNumber(token);
                    }
                    catch
                    {
                        userNumber = string.Empty;
                    }
                }

                var isSuperAdmin = false;
                if (!string.IsNullOrWhiteSpace(userNumber))
                {
                    var admin = administratorRepository.GetFirst(a => a.Number == userNumber && a.IsDelete != 1);
                    isSuperAdmin = admin != null && admin.IsSuperAdmin == 1;
                }

                if (isSuperAdmin)
                {
                    filteredMenus = allMenus;
                    var allPermNumbers = permissionRepository.AsQueryable()
                        .Select(p => p.PermissionNumber)
                        .ToList()
                        .Where(p => !p.IsNullOrEmpty())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    menuPermMap = BuildDefaultMenuPermissionMap(filteredMenus, allPermNumbers);
                    return BuildMenuTree(filteredMenus, menuPermMap);
                }

                if (string.IsNullOrWhiteSpace(userNumber))
                {
                    return BuildMenuTree(filteredMenus, menuPermMap);
                }

                // 2) 用户 -> 角色
                var roleNumbers = userRoleRepository.AsQueryable()
                    .Where(ur => ur.UserNumber == userNumber)
                    .Select(ur => ur.RoleNumber)
                    .ToList();

                if (roleNumbers == null || roleNumbers.Count == 0)
                {
                    return BuildMenuTree(filteredMenus, menuPermMap);
                }

                // 3) 角色 -> 授权（菜单与权限独立）
                var roleGrants = rolePermissionRepository.AsQueryable()
                    .Where(rp => roleNumbers.Contains(rp.RoleNumber))
                    .Select(rp => new { rp.MenuId, rp.PermissionNumber })
                    .ToList();

                var grantedPermNumbers = roleGrants
                    .Where(x => !x.PermissionNumber.IsNullOrEmpty())
                    .Select(x => x.PermissionNumber!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var directMenuIds = roleGrants
                    .Where(x => x.MenuId.HasValue && x.MenuId.Value > 0)
                    .Select(x => x.MenuId!.Value)
                    .Distinct()
                    .ToHashSet();

                if (directMenuIds.Count == 0)
                {
                    return BuildMenuTree(filteredMenus, menuPermMap);
                }

                // 4) 保留已授权菜单并补齐其父级，保证树连通
                var menuById = allMenus.ToDictionary(m => m.Id);
                var allowedMenuIds = BuildAllowedMenuIdsWithAncestors(menuById, directMenuIds);
                filteredMenus = allMenus.Where(m => allowedMenuIds.Contains(m.Id)).ToList();

                // 5) 构建按菜单聚合的权限映射
                var menuSpecificPerms = roleGrants
                    .Where(x => x.MenuId.HasValue && x.MenuId.Value > 0 && !x.PermissionNumber.IsNullOrEmpty())
                    .GroupBy(x => x.MenuId!.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.PermissionNumber!)
                              .Distinct(StringComparer.OrdinalIgnoreCase)
                              .ToList());

                if (menuSpecificPerms.Count > 0)
                {
                    menuPermMap = filteredMenus
                        .Where(m => !string.IsNullOrWhiteSpace(m.Key))
                        .ToDictionary(
                            m => m.Key!,
                            m => menuSpecificPerms.TryGetValue(m.Id, out var perms)
                                ? perms
                                : new List<string>());
                }
                else
                {
                    // 独立授权模式：将已授权权限作为可见菜单的全局权限集
                    menuPermMap = BuildDefaultMenuPermissionMap(filteredMenus, grantedPermNumbers);
                }
            }
            catch
            {
                filteredMenus = new List<Menu>();
                menuPermMap = new Dictionary<string, List<string>>();
            }

            return BuildMenuTree(filteredMenus, menuPermMap);
        }

        private static HashSet<int> BuildAllowedMenuIdsWithAncestors(
            Dictionary<int, Menu> menuById,
            HashSet<int> directMenuIds)
        {
            var allowedMenuIds = new HashSet<int>();

            foreach (var menuId in directMenuIds)
            {
                if (!menuById.TryGetValue(menuId, out var current))
                {
                    continue;
                }

                while (current != null && allowedMenuIds.Add(current.Id))
                {
                    if (current.Parent.HasValue && menuById.TryGetValue(current.Parent.Value, out var parentMenu))
                    {
                        current = parentMenu;
                    }
                    else
                    {
                        current = null;
                    }
                }
            }

            return allowedMenuIds;
        }

        private static Dictionary<string, List<string>> BuildDefaultMenuPermissionMap(
            List<Menu> menus,
            List<string> permissionNumbers)
        {
            var normalizedPerms = (permissionNumbers ?? new List<string>())
                .Where(p => !p.IsNullOrEmpty())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return menus
                .Where(m => !string.IsNullOrWhiteSpace(m.Key))
                .ToDictionary(m => m.Key!, _ => normalizedPerms.ToList());
        }

        public ListOutputDto<ReadMenuOutputDto> SelectMenuAll(ReadMenuInputDto readMenuInputDto)
        {
            var where = SqlFilterBuilder.BuildExpression<Menu, ReadMenuInputDto>(readMenuInputDto ?? new ReadMenuInputDto());

            var count = 0;
            List<Menu> allMenus = new List<Menu>();

            if (!readMenuInputDto.IgnorePaging)
            {
                allMenus = menuRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readMenuInputDto.Page, readMenuInputDto.PageSize, ref count);
            }
            else
            {
                allMenus = menuRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = allMenus.Count;
            }

            List<ReadMenuOutputDto> result = EntityMapper.MapList<Menu, ReadMenuOutputDto>(allMenus);

            return new ListOutputDto<ReadMenuOutputDto>
            {
                Data = new PagedData<ReadMenuOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 插入菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public BaseResponse InsertMenu(CreateMenuInputDto menu)
        {
            try
            {
                if (menuRepository.IsAny(a => a.Key == menu.Key && a.IsDelete == 0))
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("This menu already exists.", "菜单已存在。"), Code = BusinessStatusCode.InternalServerError };
                }

                menuRepository.Insert(EntityMapper.Map<CreateMenuInputDto, Menu>(menu));
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "插入菜单失败");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public BaseResponse UpdateMenu(UpdateMenuInputDto menu)
        {
            try
            {
                var result = menuRepository.Update(EntityMapper.Map<UpdateMenuInputDto, Menu>(menu));
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "更新菜单失败");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public BaseResponse DeleteMenu(DeleteMenuInputDto input)
        {
            try
            {
                if (input?.DelIds == null || !input.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(input);
                var menus = menuRepository.GetList(a => delIds.Contains(a.Id));

                if (!menus.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Menu Information Not Found", "菜单信息未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(input, menus, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var result = menuRepository.SoftDeleteRange(menus);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除菜单失败");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 递归构建菜单树
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private ListOutputDto<MenuDto> BuildMenuTree(List<Menu> menus, Dictionary<string, List<string>> menuPermMap)
        {
            try
            {
                var menuDict = menus.ToDictionary(m => m.Id);
                var rootNodes = new List<MenuDto>();

                var processedIds = new HashSet<int?>();

                foreach (var menu in menus.Where(m => m.Parent == null))
                {
                    var node = BuildTreeNode(menu, menuDict, processedIds, menuPermMap);
                    if (node != null) rootNodes.Add(node);
                }

                return new ListOutputDto<MenuDto>
                {
                    Code = 0,
                    Message = "菜单树构建成功",
                    Data = new PagedData<MenuDto>
                    {
                        Items = rootNodes,
                        TotalCount = rootNodes.Count
                    }
                };
            }
            catch (Exception ex)
            {
                return new ListOutputDto<MenuDto>
                {
                    Code = 5001,
                    Message = $"菜单树构建失败: {ex.Message}",
                    Data = null
                };
            }
        }

        private MenuDto BuildTreeNode(
            Menu menu,
            Dictionary<int, Menu> menuDict,
            HashSet<int?> processedIds,
            Dictionary<string, List<string>> menuPermMap)
        {
            processedIds.Add(menu.Id);

            var node = new MenuDto
            {
                Key = menu.Key,
                Title = menu.Title,
                Path = menu.Path,
                Icon = menu.Icon,
                Permissions = (menu.Key != null && menuPermMap != null && menuPermMap.TryGetValue(menu.Key, out var perms))
                    ? perms
                    : new List<string>(),
                Children = new List<MenuDto>()
            };

            var childMenus = menuDict.Values
                .Where(m => m.Parent == menu.Id)
                .ToList();

            foreach (var child in childMenus)
            {
                var childNode = BuildTreeNode(child, menuDict, processedIds, menuPermMap);
                if (childNode != null)
                {
                    node.Children.Add(childNode);
                }
            }
            return node;
        }
    }
}
