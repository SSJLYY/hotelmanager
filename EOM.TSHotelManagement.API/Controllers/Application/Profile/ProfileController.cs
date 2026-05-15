using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 个人中心控制器
    /// </summary>
    public class ProfileController(
        IProfileService profileService,
        JwtTokenRevocationService tokenRevocationService,
        ILogger<ProfileController> logger) : ControllerBase
    {
        private readonly IProfileService _profileService = profileService;
        private readonly JwtTokenRevocationService _tokenRevocationService = tokenRevocationService;
        private readonly ILogger<ProfileController> _logger = logger;

        /// <summary>
        /// 获取当前登录人基础资料
        /// </summary>
        [HttpGet]
        public SingleOutputDto<CurrentProfileOutputDto> GetCurrentProfile()
        {
            return _profileService.GetCurrentProfile(GetCurrentSerialNumber());
        }

        /// <summary>
        /// 上传当前登录人头像
        /// </summary>
        [HttpPost]
        public async Task<SingleOutputDto<UploadAvatarOutputDto>> UploadAvatar([FromForm] UploadAvatarInputDto inputDto, IFormFile file)
        {
            return await _profileService.UploadAvatar(GetCurrentSerialNumber(), inputDto, file);
        }

        /// <summary>
        /// 修改当前登录人密码
        /// </summary>
        [HttpPost]
        public BaseResponse ChangePassword([FromBody] ChangePasswordInputDto inputDto)
        {
            var response = _profileService.ChangePassword(GetCurrentSerialNumber(), inputDto);
            if (!response.Success)
            {
                return response;
            }

            try
            {
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (JwtTokenRevocationService.TryGetBearerToken(authorizationHeader, out var token))
                {
                    Task.Run(async () => { await _tokenRevocationService.RevokeTokenAsync(token); });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to revoke current token after password change.");
            }

            return response;
        }

        private string GetCurrentSerialNumber()
        {
            return User.FindFirstValue(ClaimTypes.SerialNumber)
                   ?? User.FindFirst("serialnumber")?.Value
                   ?? string.Empty;
        }
    }
}
