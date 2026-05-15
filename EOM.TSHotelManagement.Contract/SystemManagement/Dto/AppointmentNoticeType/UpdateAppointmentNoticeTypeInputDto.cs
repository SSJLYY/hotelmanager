using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateAppointmentNoticeTypeInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "公告类型编号为必填字段")]
        [MaxLength(128, ErrorMessage = "公告类型编号长度不超过128字符")]
        public string NoticeTypeNumber { get; set; }

        [Required(ErrorMessage = "公告类型名称为必填字段")]
        [MaxLength(200, ErrorMessage = "公告类型名称长度不超过200字符")]
        public string NoticeTypeName { get; set; }
    }
}
