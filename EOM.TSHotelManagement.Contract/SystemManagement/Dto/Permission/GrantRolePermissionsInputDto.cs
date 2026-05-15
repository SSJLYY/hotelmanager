using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission
{
    public class GrantRolePermissionsInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "角色编码为必填字段")]
        [MaxLength(128, ErrorMessage = "角色编码长度不超过128字符")]
        public string RoleNumber { get; set; }

        [Required(ErrorMessage = "权限编码集合为必填字段")]
        public List<string> PermissionNumbers { get; set; } = new List<string>();

        public List<int> MenuIds { get; set; } = new List<int>();
    }
}
