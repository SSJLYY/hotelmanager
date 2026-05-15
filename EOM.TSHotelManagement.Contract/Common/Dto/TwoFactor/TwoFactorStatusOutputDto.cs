namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 2FA 状态输出
    /// </summary>
    public class TwoFactorStatusOutputDto : BaseOutputDto
    {
        /// <summary>
        /// 是否已启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 启用时间
        /// </summary>
        public DateTime? EnabledAt { get; set; }

        /// <summary>
        /// 最近一次验证时间
        /// </summary>
        public DateTime? LastVerifiedAt { get; set; }

        /// <summary>
        /// 剩余可用恢复备用码数量
        /// </summary>
        public int RemainingRecoveryCodes { get; set; }
    }
}
