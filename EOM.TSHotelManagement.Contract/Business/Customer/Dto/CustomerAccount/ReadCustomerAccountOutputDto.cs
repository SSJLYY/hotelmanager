namespace EOM.TSHotelManagement.Contract
{
    public class ReadCustomerAccountOutputDto : BaseOutputDto
    {
        /// <summary>
        /// 账号 (Account)
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 邮箱 (Email)
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// 名称 (Name)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码 (Password)
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 状态 (Status)
        /// </summary>
        public int Status { get; set; } = 0;
        /// <summary>
        /// 最后一次登录地址 (Last Login IP)
        /// </summary>
        public string LastLoginIp { get; set; }
        /// <summary>
        /// 最后一次登录时间 (Last Login Time)
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 是否需要2FA
        /// </summary>
        public bool RequiresTwoFactor { get; set; }

        /// <summary>
        /// 本次登录是否通过恢复备用码完成 2FA
        /// </summary>
        public bool UsedRecoveryCodeLogin { get; set; }
    }
}
