using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission;

namespace EOM.TSHotelManagement.Service
{
    /// filename OR language.declaration()
    /// EmployeePermissionService.cs:1
    /// <summary>
    /// 员工组权限分配服务（与管理员一致的 5 个接口逻辑，校验目标用户为 Employee.EmployeeId）
    /// </summary>
    public interface IEmployeePermissionService
    {
        /// filename OR language.declaration()
        /// IEmployeePermissionService.AssignUserRoles():1
        /// <summary>为员工分配角色（全量覆盖）</summary>
        BaseResponse AssignUserRoles(AssignUserRolesInputDto input);

        /// filename OR language.declaration()
        /// IEmployeePermissionService.ReadUserRoles():1
        /// <summary>读取员工已分配的角色编码集合</summary>
        ListOutputDto<string> ReadUserRoles(string userNumber);

        /// filename OR language.declaration()
        /// IEmployeePermissionService.ReadUserRolePermissions():1
        /// <summary>读取员工“角色-权限”明细</summary>
        ListOutputDto<UserRolePermissionOutputDto> ReadUserRolePermissions(string userNumber);

        /// filename OR language.declaration()
        /// IEmployeePermissionService.AssignUserPermissions():1
        /// <summary>为员工分配“直接权限”（R-USER-{UserNumber} 全量覆盖）</summary>
        BaseResponse AssignUserPermissions(AssignUserPermissionsInputDto input);

        /// filename OR language.declaration()
        /// IEmployeePermissionService.ReadUserDirectPermissions():1
        /// <summary>读取员工“直接权限”权限编码集合（来自 R-USER-{UserNumber}）</summary>
        ListOutputDto<string> ReadUserDirectPermissions(string userNumber);
    }

}
