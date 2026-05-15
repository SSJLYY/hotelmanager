using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateNationInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "民族编号为必填字段")]
        [MaxLength(128, ErrorMessage = "民族编号长度不超过128字符")]
        public string NationNumber { get; set; }

        [Required(ErrorMessage = "民族名称为必填字段")]
        [MaxLength(50, ErrorMessage = "民族名称长度不超过50字符")]
        public string NationName { get; set; }
    }
}



