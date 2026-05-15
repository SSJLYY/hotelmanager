using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Role;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 角色服务接口
    /// </summary>
    public interface IRoleAppService
    {
        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="readRoleInputDto"></param>
        /// <returns></returns>
        ListOutputDto<ReadRoleOutputDto> SelectRoleList(ReadRoleInputDto readRoleInputDto);
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="createRoleInputDto"></param>
        /// <returns></returns>
        BaseResponse InsertRole(CreateRoleInputDto createRoleInputDto);
        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="updateRoleInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdateRole(UpdateRoleInputDto updateRoleInputDto);
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="deleteRoleInputDto"></param>
        /// <returns></returns>
        BaseResponse DeleteRole(DeleteRoleInputDto deleteRoleInputDto);
        /// <summary>
        /// 为角色授予权限（全量覆盖）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseResponse GrantRolePermissions(GrantRolePermissionsInputDto input);

        /// <summary>
        /// 读取指定角色已授予的权限编码集合
        /// </summary>
        /// <param name="roleNumber">角色编码</param>
        /// <returns>权限编码集合</returns>
        ListOutputDto<string> ReadRolePermissions(string roleNumber);

        /// <summary>
        /// 读取指定角色已授予的菜单与权限（菜单和权限独立）
        /// </summary>
        /// <param name="roleNumber">角色编码</param>
        /// <returns>角色授权明细</returns>
        SingleOutputDto<ReadRoleGrantOutputDto> ReadRoleGrants(string roleNumber);

        /// <summary>
        /// 读取隶属于指定角色的管理员用户编码集合
        /// </summary>
        /// <param name="roleNumber">角色编码</param>
        /// <returns>管理员用户编码集合</returns>
        ListOutputDto<string> ReadRoleUsers(string roleNumber);

        /// <summary>
        /// 为角色分配管理员（全量覆盖）
        /// </summary>
        /// <param name="input">包含角色编码与管理员编码集合</param>
        /// <returns>操作结果</returns>
        BaseResponse AssignRoleUsers(AssignRoleUsersInputDto input);
    }
}
