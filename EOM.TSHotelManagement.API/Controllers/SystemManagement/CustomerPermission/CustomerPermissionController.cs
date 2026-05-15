using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// filename OR language.declaration()
    /// CustomerController.CustomerController():1
    /// <summary>
    /// 客户组权限分配接口（与管理员一致的 5 个接口）
    /// 前端将调用：
    /// - POST /CustomerPermission/AssignUserRoles
    /// - POST /CustomerPermission/ReadUserRoles
    /// - POST /CustomerPermission/ReadUserRolePermissions
    /// - POST /CustomerPermission/AssignUserPermissions
    /// - POST /CustomerPermission/ReadUserDirectPermissions
    /// </summary>
    public class CustomerPermissionController : ControllerBase
    {
        private readonly ICustomerPermissionService customerPermService;

        public CustomerPermissionController(ICustomerPermissionService customerPermService)
        {
            this.customerPermService = customerPermService;
        }

        /// filename OR language.declaration()
        /// CustomerController.AssignUserRoles():1
        /// <summary>
        /// 为客户分配角色（全量覆盖）
        /// </summary>
        [RequirePermission("system:user:customer:aur")]
        [HttpPost]
        public BaseResponse AssignUserRoles([FromBody] AssignUserRolesInputDto input)
        {
            return customerPermService.AssignUserRoles(input);
        }

        /// filename OR language.declaration()
        /// CustomerController.ReadUserRoles():1
        /// <summary>
        /// 读取客户已分配的角色编码集合
        /// </summary>
        [RequirePermission("system:user:customer.rur")]
        [HttpPost]
        public ListOutputDto<string> ReadUserRoles([FromBody] ReadByUserNumberInputDto input)
        {
            return customerPermService.ReadUserRoles(input.UserNumber);
        }

        /// filename OR language.declaration()
        /// CustomerController.ReadUserRolePermissions():1
        /// <summary>
        /// 读取客户“角色-权限”明细
        /// </summary>
        [RequirePermission("system:user:customer.rurp")]
        [HttpPost]
        public ListOutputDto<UserRolePermissionOutputDto> ReadUserRolePermissions([FromBody] ReadByUserNumberInputDto input)
        {
            return customerPermService.ReadUserRolePermissions(input.UserNumber);
        }

        /// filename OR language.declaration()
        /// CustomerController.AssignUserPermissions():1
        /// <summary>
        /// 为客户分配“直接权限”（R-USER-{UserNumber} 全量覆盖）
        /// </summary>
        [RequirePermission("system:user:customer:aup")]
        [HttpPost]
        public BaseResponse AssignUserPermissions([FromBody] AssignUserPermissionsInputDto input)
        {
            return customerPermService.AssignUserPermissions(input);
        }

        /// filename OR language.declaration()
        /// CustomerController.ReadUserDirectPermissions():1
        /// <summary>
        /// 读取客户“直接权限”权限编码集合（来自 R-USER-{UserNumber}）
        /// </summary>
        [RequirePermission("system:user:customer.rudp")]
        [HttpPost]
        public ListOutputDto<string> ReadUserDirectPermissions([FromBody] ReadByUserNumberInputDto input)
        {
            return customerPermService.ReadUserDirectPermissions(input.UserNumber);
        }
    }
}
