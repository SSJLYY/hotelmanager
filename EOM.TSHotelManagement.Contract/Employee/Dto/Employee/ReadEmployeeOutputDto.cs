namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeOutputDto : BaseOutputDto
    {
        public string EmployeeId { get; set; }

        public string Name { get; set; }

        public int Gender { get; set; }

        public string GenderName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Ethnicity { get; set; }

        public string EthnicityName { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }
        public string PositionName { get; set; }

        public int? IdCardType { get; set; }
        public string IdCardTypeName { get; set; }
        public string IdCardNumber { get; set; }

        public DateTime HireDate { get; set; }
        public string PoliticalAffiliation { get; set; }
        public string PoliticalAffiliationName { get; set; }
        public string EducationLevel { get; set; }
        public string EducationLevelName { get; set; }

        public int IsEnable { get; set; }

        public int IsInitialize { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string PhotoUrl { get; set; }
        public bool RequiresTwoFactor { get; set; }

        /// <summary>
        /// 本次登录是否通过恢复备用码完成 2FA
        /// </summary>
        public bool UsedRecoveryCodeLogin { get; set; }
    }
}
