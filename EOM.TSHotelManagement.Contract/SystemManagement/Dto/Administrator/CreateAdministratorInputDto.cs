using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateAdministratorInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "管理员账号为必填字段")]
        [MaxLength(128, ErrorMessage = "管理员账号长度不超过128字符")]
        public string Number { get; set; }

        [Required(ErrorMessage = "管理员账号为必填字段")]
        [MaxLength(128, ErrorMessage = "管理员账号长度不超过128字符")]
        public string Account { get; set; }

        [Required(ErrorMessage = "管理员密码为必填字段")]
        [MaxLength(256, ErrorMessage = "管理员密码长度不超过256字符")]
        public string Password { get; set; }

        [Required(ErrorMessage = "管理员类型为必填字段")]
        [MaxLength(150, ErrorMessage = "管理员类型长度不超过150字符")]
        public string Type { get; set; }

        [Required(ErrorMessage = "管理员名称为必填字段")]
        [MaxLength(200, ErrorMessage = "管理员名称长度不超过200字符")]
        public string Name { get; set; }

        [Required(ErrorMessage = "是否为超级管理员为必填字段")]
        public int IsSuperAdmin { get; set; }
    }
}


