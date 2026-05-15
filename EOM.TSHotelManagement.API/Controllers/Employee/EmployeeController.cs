using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Infrastructure;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 员工信息控制器
    /// </summary>
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService workerService;
        private readonly JwtConfig _jwtConfig;

        public EmployeeController(IEmployeeService workerService, IOptions<JwtConfig> jwtConfig)
        {
            this.workerService = workerService;
            _jwtConfig = jwtConfig.Value;
        }

        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.ue")]
        [HttpPost]
        public BaseResponse UpdateEmployee([FromBody] UpdateEmployeeInputDto worker)
        {
            return workerService.UpdateEmployee(worker);
        }

        /// <summary>
        /// 员工账号禁/启用
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.mea")]
        [HttpPost]
        public BaseResponse ManagerEmployeeAccount([FromBody] UpdateEmployeeInputDto worker)
        {
            return workerService.ManagerEmployeeAccount(worker);
        }

        /// <summary>
        /// 添加员工信息
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.ae")]
        [HttpPost]
        public BaseResponse AddEmployee([FromBody] CreateEmployeeInputDto worker)
        {
            return workerService.AddEmployee(worker);
        }

        /// <summary>
        /// 获取所有工作人员信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.sea")]
        [HttpGet]
        public ListOutputDto<ReadEmployeeOutputDto> SelectEmployeeAll([FromQuery] ReadEmployeeInputDto inputDto)
        {
            return workerService.SelectEmployeeAll(inputDto);
        }

        /// <summary>
        /// 根据登录名称查询员工信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.seibei")]
        [HttpGet]
        public SingleOutputDto<ReadEmployeeOutputDto> SelectEmployeeInfoByEmployeeId([FromQuery] ReadEmployeeInputDto inputDto)
        {
            return workerService.SelectEmployeeInfoByEmployeeId(inputDto);
        }

        /// <summary>
        /// 根据登录名称、密码查询员工信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public SingleOutputDto<ReadEmployeeOutputDto> EmployeeLogin([FromBody] EmployeeLoginDto inputDto)
        {
            var result = workerService.EmployeeLogin(inputDto);

            // 如果登录成功，设置Refresh Token到HttpOnly Cookie
            if (result.Code == BusinessStatusCode.Success && result.Data != null && !string.IsNullOrWhiteSpace(result.Data.RefreshToken))
            {
                Response.Cookies.Append("refreshToken", result.Data.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(_jwtConfig.RefreshTokenExpiryDays) // 与Refresh Token过期时间一致
                });
            }

            return result;
        }

        /// <summary>
        /// 获取当前员工账号的 2FA 状态
        /// </summary>
        /// <returns></returns>
        [RequirePermission("staffmanagement.gtfse")]
        [HttpGet]
        public SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus()
        {
            return workerService.GetTwoFactorStatus(GetCurrentSerialNumber());
        }

        /// <summary>
        /// 生成当前员工账号的 2FA 绑定信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("staffmanagement.gtfs")]
        [HttpPost]
        public SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup()
        {
            return workerService.GenerateTwoFactorSetup(GetCurrentSerialNumber());
        }

        /// <summary>
        /// 启用当前员工账号 2FA
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.etf")]
        [HttpPost]
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return workerService.EnableTwoFactor(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 关闭当前员工账号 2FA
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.dtf")]
        [HttpPost]
        public BaseResponse DisableTwoFactor([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return workerService.DisableTwoFactor(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 重置当前员工账号恢复备用码
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.rtfrc")]
        [HttpPost]
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return workerService.RegenerateTwoFactorRecoveryCodes(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 修改员工账号密码
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.ueap")]
        [HttpPost]
        public BaseResponse UpdateEmployeeAccountPassword([FromBody] UpdateEmployeeInputDto updateEmployeeInputDto)
        {
            return workerService.UpdateEmployeeAccountPassword(updateEmployeeInputDto);
        }
        /// <summary>
        /// 重置员工账号密码
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.reap")]
        [HttpPost]
        public BaseResponse ResetEmployeeAccountPassword([FromBody] UpdateEmployeeInputDto updateEmployeeInputDto)
        {
            return workerService.ResetEmployeeAccountPassword(updateEmployeeInputDto);
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
