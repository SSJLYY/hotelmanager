using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdatePositionInputDto : BaseInputDto
    {
        public int PositionId { get; set; }

        [Required(ErrorMessage = "职位编号为必填字段")]
        [MaxLength(128, ErrorMessage = "职位编号长度不超过128字符")]
        public string PositionNumber { get; set; }

        [Required(ErrorMessage = "职位名称为必填字段")]
        [MaxLength(200, ErrorMessage = "职位名称长度不超过200字符")]
        public string PositionName { get; set; }

        public string PositionDescription { get; set; }
    }
}



