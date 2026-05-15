using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateMenuInputDto : BaseInputDto
    {
        [MaxLength(256, ErrorMessage = "菜单键长度不超过256字符")]
        public string Key { get; set; }

        [Required(ErrorMessage = "菜单标题为必填字段")]
        [MaxLength(256, ErrorMessage = "菜单标题长度不超过256字符")]
        public string Title { get; set; }

        public string Path { get; set; }
        public int? Parent { get; set; }

        [MaxLength(256, ErrorMessage = "菜单图标长度不超过256字符")]
        public string Icon { get; set; }
    }
}


