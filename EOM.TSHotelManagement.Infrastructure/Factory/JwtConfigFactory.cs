using Microsoft.Extensions.Configuration;

namespace EOM.TSHotelManagement.Infrastructure
{
    public class JwtConfigFactory
    {
        private readonly IConfiguration _configuration;

        public JwtConfigFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtConfig GetJwtConfig()
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            {
                throw new InvalidOperationException("JWT密钥长度不足，必须至少32个字符");
            }
            var jwtConfig = new JwtConfig
            {
                Key = key,
                ExpiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"]),
                RefreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7")
            };
            return jwtConfig;
        }
    }
}
