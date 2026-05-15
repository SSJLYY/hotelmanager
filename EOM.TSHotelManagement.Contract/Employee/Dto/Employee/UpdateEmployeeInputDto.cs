using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateEmployeeInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "员工账号为必填字段")]
        [MaxLength(128, ErrorMessage = "员工账号长度不超过128字符")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "员工姓名为必填字段")]
        [MaxLength(250, ErrorMessage = "员工姓名长度不超过250字符")]
        public string Name { get; set; }

        [Required(ErrorMessage = "员工性别为必填字段")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "出生日期为必填字段")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "民族类型为必填字段")]
        [MaxLength(128, ErrorMessage = "民族类型长度不超过128字符")]
        public string Ethnicity { get; set; }

        [Required(ErrorMessage = "员工电话为必填字段")]
        [MaxLength(256, ErrorMessage = "员工电话长度不超过256字符")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "所属部门为必填字段")]
        [MaxLength(128, ErrorMessage = "所属部门长度不超过128字符")]
        public string Department { get; set; }

        [MaxLength(500, ErrorMessage = "居住地址长度不超过500字符")]
        public string Address { get; set; }

        [Required(ErrorMessage = "员工职位为必填字段")]
        [MaxLength(128, ErrorMessage = "员工职位长度不超过128字符")]
        public string Position { get; set; }

        [Required(ErrorMessage = "证件类型为必填字段")]
        public int? IdCardType { get; set; }

        [Required(ErrorMessage = "证件号码为必填字段")]
        [MaxLength(256, ErrorMessage = "证件号码长度不超过256字符")]
        public string IdCardNumber { get; set; }

        [Required(ErrorMessage = "员工入职时间为必填字段")]
        public DateOnly HireDate { get; set; }

        [Required(ErrorMessage = "员工面貌为必填字段")]
        [MaxLength(128, ErrorMessage = "员工面貌长度不超过128字符")]
        public string PoliticalAffiliation { get; set; }

        [Required(ErrorMessage = "教育程度为必填字段")]
        [MaxLength(128, ErrorMessage = "教育程度长度不超过128字符")]
        public string EducationLevel { get; set; }

        [MaxLength(256, ErrorMessage = "旧密码长度不超过256字符")]
        public string OldPassword { get; set; }

        [MaxLength(256, ErrorMessage = "员工密码长度不超过256字符")]
        public string Password { get; set; }

        public int IsEnable { get; set; }
        public int IsInitialize { get; set; }

        [Required(ErrorMessage = "邮箱地址为必填字段")]
        [MaxLength(256, ErrorMessage = "邮箱地址长度不超过256字符")]
        public string EmailAddress { get; set; }
    }
}


