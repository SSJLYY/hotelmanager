using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateEducationInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "学历编号为必填字段")]
        [MaxLength(128, ErrorMessage = "学历编号长度不超过128字符")]
        public string EducationNumber { get; set; }

        [Required(ErrorMessage = "学历名称为必填字段")]
        [MaxLength(200, ErrorMessage = "学历名称长度不超过200字符")]
        public string EducationName { get; set; }
    }
}


