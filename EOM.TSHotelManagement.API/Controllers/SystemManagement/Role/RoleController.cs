using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Role;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    public class RoleController : ControllerBase
    {
        private readonly IRoleAppService _roleAppService;

        public RoleController(IRoleAppService roleAppService)
        {
            _roleAppService = roleAppService;
        }

        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="readRoleInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:role:srl")]
        [HttpGet]
        public ListOutputDto<ReadRoleOutputDto> SelectRoleList([FromQuery] ReadRoleInputDto readRoleInputDto)
        {
            return _roleAppService.SelectRoleList(readRoleInputDto);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="createRoleInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:role:insertrole")]
        [HttpPost]
        public BaseResponse InsertRole([FromBody] CreateRoleInputDto createRoleInputDto)
        {
            return _roleAppService.InsertRole(createRoleInputDto);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="updateRoleInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:role:updaterole")]
        [HttpPost]
        public BaseResponse UpdateRole([FromBody] UpdateRoleInputDto updateRoleInputDto)
        {
            return _roleAppService.UpdateRole(updateRoleInputDto);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="deleteRoleInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:role:deleterole")]
        [HttpPost]
        public BaseResponse DeleteRole([FromBody] DeleteRoleInputDto deleteRoleInputDto)
        {
            return _roleAppService.DeleteRole(deleteRoleInputDto);
        }

        /// <summary>
        /// 为角色授予权限（全量覆盖）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequirePermission("system:role:grp")]
        [HttpPost]
        public BaseResponse GrantRolePermissions([FromBody] GrantRolePermissionsInputDto input)
        {
            return _roleAppService.GrantRolePermissions(input);
        }

        /// <summary>
        /// 读取指定角色已授予的权限编码集合
        /// </summary>
        /// <param name="input">角色编码请求体</param>
        [RequirePermission("system:role:rrp")]
        [HttpPost]
        public ListOutputDto<string> ReadRolePermissions([FromBody] ReadByRoleNumberInputDto input)
        {
            return _roleAppService.ReadRolePermissions(input.RoleNumber);
        }

        /// <summary>
        /// 读取指定角色菜单和权限授权（菜单与权限独立）
        /// </summary>
        [RequirePermission("system:role:rrg")]
        [HttpPost]
        public SingleOutputDto<ReadRoleGrantOutputDto> ReadRoleGrants([FromBody] ReadByRoleNumberInputDto input)
        {
            return _roleAppService.ReadRoleGrants(input.RoleNumber);
        }

        /// <summary>
        /// 读取隶属于指定角色的管理员用户编码集合
        /// </summary>
        /// <param name="input">角色编码请求体</param>
        [RequirePermission("system:role:rru")]
        [HttpPost]
        public ListOutputDto<string> ReadRoleUsers([FromBody] ReadByRoleNumberInputDto input)
        {
            return _roleAppService.ReadRoleUsers(input.RoleNumber);
        }

        /// <summary>
        /// 为角色分配管理员（全量覆盖）
        /// </summary>
        /// <param name="input">包含角色编码与管理员编码集合</param>
        [RequirePermission("system:role:aru")]
        [HttpPost]
        public BaseResponse AssignRoleUsers([FromBody] AssignRoleUsersInputDto input)
        {
            return _roleAppService.AssignRoleUsers(input);
        }
    }
}
