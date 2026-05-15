namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 2FA 绑定信息输出
    /// </summary>
    public class TwoFactorSetupOutputDto : BaseOutputDto
    {
        /// <summary>
        /// 是否已启用 2FA
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 账号标识（Authenticator 展示）
        /// </summary>
        public string AccountName { get; set; } = string.Empty;

        /// <summary>
        /// 手动录入密钥（Base32）
        /// </summary>
        public string ManualEntryKey { get; set; } = string.Empty;

        /// <summary>
        /// otpauth URI
        /// </summary>
        public string OtpAuthUri { get; set; } = string.Empty;

        /// <summary>
        /// 验证码位数
        /// </summary>
        public int CodeDigits { get; set; }

        /// <summary>
        /// 时间步长（秒）
        /// </summary>
        public int TimeStepSeconds { get; set; }
    }
}
