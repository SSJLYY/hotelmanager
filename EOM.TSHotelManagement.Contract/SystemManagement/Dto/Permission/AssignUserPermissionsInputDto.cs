using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class AssignUserPermissionsInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "用户编码为必填字段")]
        [MaxLength(128, ErrorMessage = "用户编码长度不超过128字符")]
        public string UserNumber { get; set; } = null!;

        [Required(ErrorMessage = "权限编码集合为必填字段")]
        public List<string> PermissionNumbers { get; set; } = new List<string>();
    }
}