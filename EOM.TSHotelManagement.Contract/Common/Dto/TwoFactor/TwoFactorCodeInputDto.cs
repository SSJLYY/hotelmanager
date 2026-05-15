namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 2FA 验证码输入
    /// </summary>
    public class TwoFactorCodeInputDto : BaseInputDto
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string VerificationCode { get; set; } = string.Empty;
    }
}
