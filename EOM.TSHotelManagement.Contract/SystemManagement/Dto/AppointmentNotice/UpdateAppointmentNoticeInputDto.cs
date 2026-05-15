using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateAppointmentNoticeInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "公告编号为必填字段")]
        [MaxLength(128, ErrorMessage = "公告编号长度不超过128字符")]
        public string NoticeId { get; set; }

        [Required(ErrorMessage = "公告标题为必填字段")]
        [MaxLength(256, ErrorMessage = "公告标题长度不超过256字符")]
        public string NoticeTitle { get; set; }

        public string NoticeContent { get; set; }

        [Required(ErrorMessage = "公告日期为必填字段")]
        public DateTime NoticeDate { get; set; }
    }
}


