using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class ChangePasswordInputDto
    {
        [Required(ErrorMessage = "旧密码为必填字段")]
        [MaxLength(256, ErrorMessage = "旧密码长度不能超过256字符")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "新密码为必填字段")]
        [MaxLength(256, ErrorMessage = "新密码长度不能超过256字符")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "确认密码为必填字段")]
        [MaxLength(256, ErrorMessage = "确认密码长度不能超过256字符")]
        public string ConfirmPassword { get; set; }
    }
}
