using Microsoft.Extensions.Configuration;

namespace EOM.TSHotelManagement.Infrastructure
{
    /// <summary>
    /// 2FA 配置工厂
    /// </summary>
    public class TwoFactorConfigFactory
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public TwoFactorConfigFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 读取 2FA 配置
        /// </summary>
        /// <returns></returns>
        public TwoFactorConfig GetTwoFactorConfig()
        {
            return new TwoFactorConfig
            {
                Issuer = _configuration.GetSection("TwoFactor").GetValue<string>("Issuer") ?? "TSHotel",
                SecretSize = _configuration.GetSection("TwoFactor").GetValue<int?>("SecretSize") ?? 20,
                CodeDigits = _configuration.GetSection("TwoFactor").GetValue<int?>("CodeDigits") ?? 6,
                TimeStepSeconds = _configuration.GetSection("TwoFactor").GetValue<int?>("TimeStepSeconds") ?? 30,
                AllowedDriftWindows = _configuration.GetSection("TwoFactor").GetValue<int?>("AllowedDriftWindows") ?? 1,
                RecoveryCodeCount = _configuration.GetSection("TwoFactor").GetValue<int?>("RecoveryCodeCount") ?? 8,
                RecoveryCodeLength = _configuration.GetSection("TwoFactor").GetValue<int?>("RecoveryCodeLength") ?? 10,
                RecoveryCodeGroupSize = _configuration.GetSection("TwoFactor").GetValue<int?>("RecoveryCodeGroupSize") ?? 5
            };
        }
    }
}
