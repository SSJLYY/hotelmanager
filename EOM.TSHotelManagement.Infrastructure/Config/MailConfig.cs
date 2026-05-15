namespace EOM.TSHotelManagement.Infrastructure
{
    public class MailConfig
    {
        /// <summary>
        /// 是否启用邮件服务
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// SMTP服务器地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// SMTP端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 发件邮箱账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 发件邮箱密码/应用专用密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 发件人显示名称
        /// </summary>
        public string DisplayName { get; set; }
    }
}
