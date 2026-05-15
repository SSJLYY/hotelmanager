using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreatePositionInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "职位编号为必填字段"), MaxLength(128, ErrorMessage = "职位编号最大长度为128字符")]
        public string PositionNumber { get; set; }
        [Required(ErrorMessage = "职位名称为必填字段"), MaxLength(200, ErrorMessage = "职位名称最大长度为200字符")]
        public string PositionName { get; set; }
    }
}



