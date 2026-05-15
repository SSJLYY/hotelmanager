namespace EOM.TSHotelManagement.Contract
{
    public class ReadAdministratorOutputDto : BaseOutputDto
    {
        public string Number { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public int IsSuperAdmin { get; set; }

        public string IsSuperAdminDescription { get; set; }

        public string TypeName { get; set; }

        public bool RequiresTwoFactor { get; set; }

        /// <summary>
        /// 本次登录是否通过恢复备用码完成 2FA
        /// </summary>
        public bool UsedRecoveryCodeLogin { get; set; }
    }
}


