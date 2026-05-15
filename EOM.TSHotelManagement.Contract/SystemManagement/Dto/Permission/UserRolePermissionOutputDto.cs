using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UserRolePermissionOutputDto : BaseOutputDto
    {
        [MaxLength(128, ErrorMessage = "角色编码长度不超过128字符")]
        public string RoleNumber { get; set; } = null!;

        [MaxLength(128, ErrorMessage = "权限编码长度不超过128字符")]
        public string PermissionNumber { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "权限名称长度不超过200字符")]
        public string? PermissionName { get; set; }

        [MaxLength(256, ErrorMessage = "菜单键长度不超过256字符")]
        public string? MenuKey { get; set; }

        public int? MenuId { get; set; }

        [MaxLength(256, ErrorMessage = "菜单名称长度不超过256字符")]
        public string? MenuName { get; set; }
    }
}
