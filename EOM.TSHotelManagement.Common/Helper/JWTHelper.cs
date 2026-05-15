using EOM.TSHotelManagement.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EOM.TSHotelManagement.Common
{
    public class JWTHelper
    {
        private readonly JwtConfigFactory _jwtConfigFactory;
        private readonly ILogger<JWTHelper> _logger;

        public JWTHelper(JwtConfigFactory jwtConfigFactory, ILogger<JWTHelper> logger)
        {
            _jwtConfigFactory = jwtConfigFactory;
            _logger = logger;
        }

        public Dictionary<string, string> ClaimTypeMappings { get; set; } = new()
        {
            { "serialnumber", ClaimTypes.SerialNumber },
            { "unique_name", ClaimTypes.Name }
        };

        private JwtConfig GetJwtConfig() => _jwtConfigFactory.GetJwtConfig();

        /// <summary>
        /// JWT 加密生成
        /// </summary>
        public string GenerateJWT(ClaimsIdentity claimsIdentity, int? ExpiryMinutes = null)
        {
            var jwtConfig = GetJwtConfig();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtConfig.Key);

            var expiryMinutes = ExpiryMinutes ?? jwtConfig.ExpiryMinutes;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.Now.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 生成 Refresh Token
        /// </summary>
        public string GenerateRefreshToken(ClaimsIdentity claimsIdentity)
        {
            var jwtConfig = GetJwtConfig();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtConfig.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.Now.AddDays(jwtConfig.RefreshTokenExpiryDays),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// JWT 解密验证（返回 Claims 主体）
        /// </summary>
        public ClaimsPrincipal ValidateAndDecryptToken(string token)
        {
            var jwtConfig = GetJwtConfig();
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
                ValidateIssuerSigningKey = false,
                ValidateIssuer = false,
                ValidateAudience = false,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "JWT token has expired.");
                throw new ApplicationException("Token 已过期");
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                _logger.LogError(ex, "JWT signature validation failed.");
                throw new ApplicationException("无效的签名");
            }
            catch (SecurityTokenMalformedException ex)
            {
                _logger.LogError(ex, "Invalid JWT format.");
                throw new ApplicationException("Token 格式错误");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "JWT token validation failed.");
                throw new ApplicationException($"Token 验证失败: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during JWT token validation.");
                throw new ApplicationException($"Token 验证失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取唯一序列号
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityTokenException"></exception>
        public string GetSerialNumber(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var claimsPrincipal = ValidateAndDecryptToken(token);

            var serialClaim = claimsPrincipal.FindFirst(ClaimTypes.SerialNumber)
                ?? claimsPrincipal.FindFirst("serialnumber");

            return serialClaim?.Value ?? throw new SecurityTokenException("无法找到序列号声明");
        }

        /// <summary>
        /// 获取所有声明信息
        /// </summary>
        public IEnumerable<Claim> GetClaims(string token)
        {
            var principal = ValidateAndDecryptToken(token);
            return principal.Claims;
        }

        /// <summary>
        /// 获取 Token 过期时间
        /// </summary>
        public DateTime GetExpirationTime(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.ValidTo.ToLocalTime();
        }
    }
}