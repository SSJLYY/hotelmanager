using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateNavBarInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "导航栏名称为必填字段"), MaxLength(50, ErrorMessage = "导航栏名称最大长度为50字符")]
        public string NavigationBarName { get; set; }
        public int NavigationBarOrder { get; set; }
        public string NavigationBarImage { get; set; }
        [Required(ErrorMessage = "导航栏事件名为必填字段"), MaxLength(200, ErrorMessage = "导航栏事件名最大长度为200字符")]
        public string NavigationBarEvent { get; set; }
        public int MarginLeft { get; set; }
    }
}

