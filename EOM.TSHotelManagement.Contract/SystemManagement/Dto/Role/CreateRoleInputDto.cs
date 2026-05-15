using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateRoleInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "角色编码为必填字段")]
        [MaxLength(128, ErrorMessage = "角色编码长度不超过128字符")]
        public string RoleNumber { get; set; } = null!;

        [Required(ErrorMessage = "角色名称为必填字段")]
        [MaxLength(200, ErrorMessage = "角色名称长度不超过200字符")]
        public string RoleName { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "角色描述长度不超过500字符")]
        public string? RoleDescription { get; set; }
    }
}


