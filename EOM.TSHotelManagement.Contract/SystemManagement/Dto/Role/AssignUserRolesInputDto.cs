using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class AssignUserRolesInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "用户编码为必填字段")]
        [MaxLength(128, ErrorMessage = "用户编码长度不超过128字符")]
        public string UserNumber { get; set; } = null!;

        [Required(ErrorMessage = "角色编码集合为必填字段")]
        public List<string> RoleNumbers { get; set; } = new List<string>();
    }
}