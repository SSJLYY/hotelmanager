using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 管理员控制器
    /// </summary>
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// 管理员模块
        /// </summary>
        private readonly IAdminService adminService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminService"></param>
        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        /// <summary>
        /// 后台系统登录
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public SingleOutputDto<ReadAdministratorOutputDto> Login([FromBody] ReadAdministratorInputDto admin)
        {
            var result = adminService.Login(admin);

            // 如果登录成功，设置Refresh Token到HttpOnly Cookie
            if (result.Code == BusinessStatusCode.Success && result.Data != null && !string.IsNullOrWhiteSpace(result.Data.RefreshToken))
            {
                Response.Cookies.Append("refreshToken", result.Data.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(7) // 与Refresh Token过期时间一致
                });
            }

            return result;
        }

        /// <summary>
        /// 获取当前管理员账号的 2FA 状态
        /// </summary>
        /// <returns></returns>
        [RequirePermission("system:admin:gtfs")]
        [HttpGet]
        public SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus()
        {
            return adminService.GetTwoFactorStatus(GetCurrentSerialNumber());
        }

        /// <summary>
        /// 生成当前管理员账号的 2FA 绑定信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("system:admin:gtfsu")]
        [HttpPost]
        public SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup()
        {
            return adminService.GenerateTwoFactorSetup(GetCurrentSerialNumber());
        }

        /// <summary>
        /// 启用当前管理员账号 2FA
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admin:etf")]
        [HttpPost]
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return adminService.EnableTwoFactor(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 关闭当前管理员账号 2FA
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admin:dtf")]
        [HttpPost]
        public BaseResponse DisableTwoFactor([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return adminService.DisableTwoFactor(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 重置当前管理员账号恢复备用码
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admin:rtfrc")]
        [HttpPost]
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return adminService.RegenerateTwoFactorRecoveryCodes(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 获取所有管理员列表
        /// </summary>
        /// <returns></returns>
        [RequirePermission("system:admin:gaal")]
        [HttpGet]
        public ListOutputDto<ReadAdministratorOutputDto> GetAllAdminList(ReadAdministratorInputDto readAdministratorInputDto)
        {
            return adminService.GetAllAdminList(readAdministratorInputDto);
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [RequirePermission("system:admin:addadmin")]
        [HttpPost]
        public BaseResponse AddAdmin([FromBody] CreateAdministratorInputDto admin)
        {
            return adminService.AddAdmin(admin);
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="updateAdministratorInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admin:updadmin")]
        [HttpPost]
        public BaseResponse UpdAdmin([FromBody] UpdateAdministratorInputDto updateAdministratorInputDto)
        {
            return adminService.UpdAdmin(updateAdministratorInputDto);
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="deleteAdministratorInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admin:deladmin")]
        [HttpPost]
        public BaseResponse DelAdmin([FromBody] DeleteAdministratorInputDto deleteAdministratorInputDto)
        {
            return adminService.DelAdmin(deleteAdministratorInputDto);
        }

        /// <summary>
        /// 获取所有管理员类型
        /// </summary>
        /// <returns></returns>
        [RequirePermission("system:admintype:gaat")]
        [HttpGet]
        public ListOutputDto<ReadAdministratorTypeOutputDto> GetAllAdminTypes(ReadAdministratorTypeInputDto readAdministratorTypeInputDto)
        {
            return adminService.GetAllAdminTypes(readAdministratorTypeInputDto);
        }

        /// <summary>
        /// 添加管理员类型
        /// </summary>
        /// <param name="createAdministratorTypeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admintype:aat")]
        [HttpPost]
        public BaseResponse AddAdminType([FromBody] CreateAdministratorTypeInputDto createAdministratorTypeInputDto)
        {
            return adminService.AddAdminType(createAdministratorTypeInputDto);
        }

        /// <summary>
        /// 更新管理员类型
        /// </summary>
        /// <param name="updateAdministratorTypeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admintype:uat")]
        [HttpPost]
        public BaseResponse UpdAdminType([FromBody] UpdateAdministratorTypeInputDto updateAdministratorTypeInputDto)
        {
            return adminService.UpdAdminType(updateAdministratorTypeInputDto);
        }

        /// <summary>
        /// 删除管理员类型
        /// </summary>
        /// <param name="deleteAdministratorTypeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("system:admintype:dat")]
        [HttpPost]
        public BaseResponse DelAdminType([FromBody] DeleteAdministratorTypeInputDto deleteAdministratorTypeInputDto)
        {
            return adminService.DelAdminType(deleteAdministratorTypeInputDto);
        }

        /// <summary>
        /// 为用户分配角色（全量覆盖）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequirePermission("system:user:admin:aur")]
        [HttpPost]
        public BaseResponse AssignUserRoles([FromBody] AssignUserRolesInputDto input)
        {
            return adminService.AssignUserRoles(input);
        }

        /// <summary>
        /// 读取指定用户已分配的角色编码集合
        /// </summary>
        /// <param name="input">用户编码请求体</param>
        /// <returns>角色编码集合（RoleNumber 列表）</returns>
        [RequirePermission("system:user:admin.rur")]
        [HttpPost]
        public ListOutputDto<string> ReadUserRoles([FromBody] ReadByUserNumberInputDto input)
        {
            return adminService.ReadUserRoles(input.UserNumber);
        }

        /// <summary>
        /// 读取指定用户的“角色-权限”明细（来自 RolePermission 关联，并联到 Permission 得到权限码与名称）
        /// </summary>
        /// <param name="input">用户编码请求体</param>
        /// <returns>明细列表（包含 RoleNumber、PermissionNumber、PermissionName、MenuKey）</returns>
        [RequirePermission("system:user:admin.rurp")]
        [HttpPost]
        public ListOutputDto<UserRolePermissionOutputDto> ReadUserRolePermissions([FromBody] ReadByUserNumberInputDto input)
        {
            return adminService.ReadUserRolePermissions(input.UserNumber);
        }

        /// <summary>
        /// 为指定用户分配“直接权限”（通过专属角色 R-USER-{UserNumber} 写入 RolePermission，全量覆盖）
        /// </summary>
        [RequirePermission("system:user:admin:aup")]
        [HttpPost]
        public BaseResponse AssignUserPermissions([FromBody] AssignUserPermissionsInputDto input)
        {
            return adminService.AssignUserPermissions(input);
        }

        /// <summary>
        /// 读取指定用户的“直接权限”（仅来自专属角色 R-USER-{UserNumber} 的权限编码列表）
        /// </summary>
        [RequirePermission("system:user:admin.rudp")]
        [HttpPost]
        public ListOutputDto<string> ReadUserDirectPermissions([FromBody] ReadByUserNumberInputDto input)
        {
            return adminService.ReadUserDirectPermissions(input.UserNumber);
        }

        /// <summary>
        /// 从当前登录上下文中读取账号序列号
        /// </summary>
        /// <returns></returns>
        private string GetCurrentSerialNumber()
        {
            return User.FindFirstValue(ClaimTypes.SerialNumber)
                   ?? User.FindFirst("serialnumber")?.Value
                   ?? string.Empty;
        }
    }
}
