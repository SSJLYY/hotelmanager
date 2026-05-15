using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadPermissionInputDto : ListInputDto
    {
        [MaxLength(128, ErrorMessage = "权限编码长度不超过128字符")]
        public string? PermissionNumber { get; set; }

        [MaxLength(200, ErrorMessage = "权限名称长度不超过200字符")]
        public string? PermissionName { get; set; }

        [MaxLength(256, ErrorMessage = "菜单键长度不超过256字符")]
        public string? MenuKey { get; set; }

        [MaxLength(128, ErrorMessage = "所属模块长度不超过128字符")]
        public string? Module { get; set; }
    }

    public class ReadPermissionOutputDto : BaseOutputDto
    {
        [MaxLength(128, ErrorMessage = "权限编码长度不超过128字符")]
        public string PermissionNumber { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "权限名称长度不超过200字符")]
        public string PermissionName { get; set; } = string.Empty;

        [MaxLength(128, ErrorMessage = "所属模块长度不超过128字符")]
        public string? Module { get; set; }

        [MaxLength(256, ErrorMessage = "菜单键长度不超过256字符")]
        public string? MenuKey { get; set; }

        [MaxLength(500, ErrorMessage = "描述长度不超过500字符")]
        public string? Description { get; set; }
    }
}