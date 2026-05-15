namespace EOM.TSHotelManagement.Infrastructure
{
    /// <summary>
    /// 2FA（TOTP）配置
    /// </summary>
    public class TwoFactorConfig
    {
        /// <summary>
        /// 签发方名称（Authenticator 展示）
        /// </summary>
        public string Issuer { get; set; } = "TSHotel";

        /// <summary>
        /// 密钥字节长度
        /// </summary>
        public int SecretSize { get; set; } = 20;

        /// <summary>
        /// 验证码位数
        /// </summary>
        public int CodeDigits { get; set; } = 6;

        /// <summary>
        /// 时间步长（秒）
        /// </summary>
        public int TimeStepSeconds { get; set; } = 30;

        /// <summary>
        /// 允许时间漂移窗口数
        /// </summary>
        public int AllowedDriftWindows { get; set; } = 1;

        /// <summary>
        /// 每次生成的恢复备用码数量
        /// </summary>
        public int RecoveryCodeCount { get; set; } = 8;

        /// <summary>
        /// 单个恢复备用码字符长度（不含分隔符）
        /// </summary>
        public int RecoveryCodeLength { get; set; } = 10;

        /// <summary>
        /// 恢复备用码分组长度（用于展示格式，如 5-5）
        /// </summary>
        public int RecoveryCodeGroupSize { get; set; } = 5;
    }
}
