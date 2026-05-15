namespace EOM.TSHotelManagement.Contract
{
    public class ReadCustomerAccountInputDto : BaseInputDto
    {
        /// <summary>
        /// 账号 (Account)
        /// </summary>
        public string? Account { get; set; }
        /// <summary>
        /// 密码 (Password)
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// 邮箱 (Email)
        /// </summary>
        public string? EmailAddress { get; set; }
        /// <summary>
        /// 二次验证码 (2FA Code)
        /// </summary>
        public string? TwoFactorCode { get; set; }
    }
}
