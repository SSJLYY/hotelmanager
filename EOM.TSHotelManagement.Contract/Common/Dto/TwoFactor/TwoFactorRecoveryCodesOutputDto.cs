namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 2FA 恢复备用码输出
    /// </summary>
    public class TwoFactorRecoveryCodesOutputDto : BaseOutputDto
    {
        /// <summary>
        /// 新生成的恢复备用码（仅返回一次）
        /// </summary>
        public List<string> RecoveryCodes { get; set; } = new();

        /// <summary>
        /// 剩余可用恢复备用码数量
        /// </summary>
        public int RemainingCount { get; set; }
    }
}
