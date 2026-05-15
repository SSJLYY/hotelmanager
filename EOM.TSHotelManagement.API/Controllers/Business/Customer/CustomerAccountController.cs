using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    [ApiExplorerSettings(GroupName = "v1_Mobile")]
    public class CustomerAccountController : ControllerBase
    {
        private readonly ICustomerAccountService _customerAccountService;

        public CustomerAccountController(ICustomerAccountService customerAccountService)
        {
            _customerAccountService = customerAccountService;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="readCustomerAccountInputDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public SingleOutputDto<ReadCustomerAccountOutputDto> Login([FromBody] ReadCustomerAccountInputDto readCustomerAccountInputDto)
        {
            var result = _customerAccountService.Login(readCustomerAccountInputDto);

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
        /// 注册
        /// </summary>
        /// <param name="readCustomerAccountInputDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public SingleOutputDto<ReadCustomerAccountOutputDto> Register([FromBody] ReadCustomerAccountInputDto readCustomerAccountInputDto)
        {
            return _customerAccountService.Register(readCustomerAccountInputDto);
        }

        /// <summary>
        /// 获取当前客户账号的 2FA 状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus()
        {
            return _customerAccountService.GetTwoFactorStatus(GetCurrentSerialNumber());
        }

        /// <summary>
        /// 生成当前客户账号的 2FA 绑定信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup()
        {
            return _customerAccountService.GenerateTwoFactorSetup(GetCurrentSerialNumber());
        }

        /// <summary>
        /// 启用当前客户账号 2FA
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return _customerAccountService.EnableTwoFactor(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 关闭当前客户账号 2FA
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse DisableTwoFactor([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return _customerAccountService.DisableTwoFactor(GetCurrentSerialNumber(), inputDto);
        }

        /// <summary>
        /// 重置当前客户账号恢复备用码
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes([FromBody] TwoFactorCodeInputDto inputDto)
        {
            return _customerAccountService.RegenerateTwoFactorRecoveryCodes(GetCurrentSerialNumber(), inputDto);
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
