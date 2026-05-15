using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateNavBarInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "导航栏ID为必填字段")]
        public int NavigationBarId { get; set; }

        [Required(ErrorMessage = "导航栏名称为必填字段"), MaxLength(50, ErrorMessage = "导航栏名称长度不超过50字符")]
        public string NavigationBarName { get; set; }

        [Required(ErrorMessage = "导航栏排序为必填字段")]
        public int NavigationBarOrder { get; set; }

        [MaxLength(255, ErrorMessage = "导航栏图片长度不超过255字符")]
        public string NavigationBarImage { get; set; }

        [Required(ErrorMessage = "导航栏事件为必填字段"), MaxLength(200, ErrorMessage = "导航栏事件长度不超过200字符")]
        public string NavigationBarEvent { get; set; }

        [Required(ErrorMessage = "左边距为必填字段")]
        public int MarginLeft { get; set; }
    }
}

