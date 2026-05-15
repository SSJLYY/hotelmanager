using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateSystemInformationInputDto : BaseInputDto
    {
        public int InformationId { get; set; }

        [MaxLength(256, ErrorMessage = "信息标题长度不超过256字符")]
        public string InformationTitle { get; set; }

        public string InformationContent { get; set; }

        [Required(ErrorMessage = "信息日期为必填字段")]
        public DateTime InformationDate { get; set; }
    }
}



