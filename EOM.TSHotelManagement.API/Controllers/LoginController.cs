using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.API.Controllers
{
    /// <summary>
    /// 认证相关接口。
    /// </summary>
    public class LoginController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;
        private readonly CsrfTokenConfig _csrfConfig;
        private readonly JwtConfig _jwtConfig;
        private readonly JwtTokenRevocationService _tokenRevocationService;
        private readonly JWTHelper _jwtHelper;

        private readonly ILogger<LoginController> _logger;

        public LoginController(
            IAntiforgery antiforgery,
            IOptions<CsrfTokenConfig> csrfConfig,
            IOptions<JwtConfig> jwtConfig,
            JwtTokenRevocationService tokenRevocationService,
            JWTHelper jwtHelper,
            ILogger<LoginController> logger)
        {
            _antiforgery = antiforgery;
            _csrfConfig = csrfConfig.Value;
            _jwtConfig = jwtConfig.Value;
            _tokenRevocationService = tokenRevocationService;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        /// <summary>
        /// 获取防跨站请求伪造令牌，并写入配置的浏览器凭据。
        /// </summary>
        /// <returns>防跨站请求伪造令牌信息。</returns>
        [HttpGet]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public SingleOutputDto<CsrfTokenDto> GetCSRFToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            var expiresAt = DateTime.Now.AddMinutes(_csrfConfig.TokenExpirationInMinutes);

            Response.Cookies.Append(_csrfConfig.CookieName, tokens.RequestToken);
            var refreshThreshold = expiresAt.AddMinutes(-_csrfConfig.TokenRefreshThresholdInMinutes);

            return new SingleOutputDto<CsrfTokenDto>
            {
                Data = new CsrfTokenDto
                {
                    Token = tokens.RequestToken ?? string.Empty,
                    ExpiresAt = expiresAt,
                    NeedsRefresh = DateTime.Now >= refreshThreshold
                }
            };
        }

        /// <summary>
        /// 刷新防跨站请求伪造令牌。
        /// </summary>
        /// <returns>新的防跨站请求伪造令牌信息。</returns>
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public SingleOutputDto<CsrfTokenDto> RefreshCSRFToken()
        {
            return GetCSRFToken();
        }

        /// <summary>
        /// 吊销当前访问令牌并完成登出。
        /// </summary>
        /// <returns>登出结果及原因。</returns>
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<SingleOutputDto<LogoutResultDto>> Logout()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var accessTokenRevoked = false;
            var refreshTokenRevoked = false;

            // 撤销Access Token
            bool accessTokenNeedsRevocation = false;
            if (JwtTokenRevocationService.TryGetBearerToken(authorizationHeader, out var accessToken))
            {
                if (!TryReadJwtToken(accessToken, out var jwtToken) || IsTokenExpired(jwtToken))
                {
                    // Token已过期或无效，但仍视为已登出
                }
                else if (await _tokenRevocationService.IsTokenRevokedAsync(accessToken))
                {
                    // Token已被撤销
                }
                else
                {
                    accessTokenNeedsRevocation = true;
                    try
                    {
                        await _tokenRevocationService.RevokeTokenAsync(accessToken);
                        accessTokenRevoked = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to revoke access token.");
                    }
                }
            }

            // 撤销Refresh Token (从Cookie中获取)
            bool refreshTokenNeedsRevocation = false;
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                if (await _tokenRevocationService.IsTokenRevokedAsync(refreshToken))
                {
                    // Refresh Token已被撤销
                }
                else
                {
                    refreshTokenNeedsRevocation = true;
                    try
                    {
                        await _tokenRevocationService.RevokeTokenAsync(refreshToken);
                        refreshTokenRevoked = true;

                        // 清除Refresh Token Cookie
                        Response.Cookies.Delete("refreshToken", new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to revoke refresh token.");
                    }
                }
            }

            // 确保需要撤销的令牌都成功撤销
            if ((accessTokenNeedsRevocation && !accessTokenRevoked) || (refreshTokenNeedsRevocation && !refreshTokenRevoked))
            {
                return BuildLogoutResponse(
                    loggedOut: false,
                    message: "Logout failed due to token revocation error.",
                    reason: "token_revocation_failed");
            }

            return BuildLogoutResponse(
                loggedOut: true,
                message: "Logout success.");
        }

        /// <summary>
        /// 刷新访问令牌
        /// </summary>
        /// <returns>新的访问令牌信息</returns>
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<SingleOutputDto<TokenRefreshResponseDto>> RefreshToken([FromBody] TokenRefreshRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return new SingleOutputDto<TokenRefreshResponseDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Refresh token is required", "需要刷新令牌")
                };
            }

            try
            {
                // 验证Refresh Token
                var principal = _jwtHelper.ValidateAndDecryptToken(request.RefreshToken);
                var employeeId = principal.FindFirst(ClaimTypes.SerialNumber)?.Value;

                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    return new SingleOutputDto<TokenRefreshResponseDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Invalid refresh token", "无效的刷新令牌")
                    };
                }

                // 检查Refresh Token是否被撤销
                if (await _tokenRevocationService.IsTokenRevokedAsync(request.RefreshToken))
                {
                    return new SingleOutputDto<TokenRefreshResponseDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Refresh token has been revoked", "刷新令牌已被撤销")
                    };
                }

                // 生成新的Access Token
                var claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, principal.FindFirst(ClaimTypes.Name)?.Value ?? ""),
                    new Claim(ClaimTypes.SerialNumber, employeeId)
                });

                var newAccessToken = _jwtHelper.GenerateJWT(claimsIdentity);
                var newRefreshToken = _jwtHelper.GenerateRefreshToken(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.SerialNumber, employeeId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }));

                // 撤销旧的Refresh Token
                await _tokenRevocationService.RevokeTokenAsync(request.RefreshToken);

                // 设置新的Refresh Token到Cookie
                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(_jwtConfig.RefreshTokenExpiryDays)
                });

                return new SingleOutputDto<TokenRefreshResponseDto>
                {
                    Data = new TokenRefreshResponseDto
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken
                    }
                };
            }
            catch (Exception)
            {
                return new SingleOutputDto<TokenRefreshResponseDto>
                {
                    Code = BusinessStatusCode.Unauthorized,
                    Message = LocalizationHelper.GetLocalizedString("Invalid refresh token", "无效的刷新令牌")
                };
            }
        }

        private bool TryReadJwtToken(string token, out JwtSecurityToken jwtToken)
        {
            jwtToken = null;
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return jwtToken != null;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse JWT token.");
                return false;
            }
        }

        private static bool IsTokenExpired(JwtSecurityToken jwtToken)
        {
            if (jwtToken == null || jwtToken.ValidTo == DateTime.MinValue)
            {
                return false;
            }

            var expiresAtUtc = DateTime.SpecifyKind(jwtToken.ValidTo, DateTimeKind.Utc);
            return expiresAtUtc <= DateTime.UtcNow;
        }

        private static SingleOutputDto<LogoutResultDto> BuildLogoutResponse(
            bool loggedOut,
            string message,
            string reason = null)
        {
            return new SingleOutputDto<LogoutResultDto>
            {
                Code = BusinessStatusCode.Success,
                Message = LocalizationHelper.GetLocalizedString(message, message),
                Data = new LogoutResultDto
                {
                    LoggedOut = loggedOut,
                    Reason = reason
                }
            };
        }
    }
}
