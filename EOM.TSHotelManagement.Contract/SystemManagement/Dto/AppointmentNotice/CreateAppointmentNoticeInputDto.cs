using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateAppointmentNoticeInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "公告编号为必填字段")]
        [MaxLength(128, ErrorMessage = "公告编号长度不超过128字符")]
        public string NoticeId { get; set; }

        [Required(ErrorMessage = "公告主题为必填字段")]
        [MaxLength(256, ErrorMessage = "公告主题长度不超过256字符")]
        public string NoticeTheme { get; set; }

        [Required(ErrorMessage = "公告类型为必填字段")]
        [MaxLength(150, ErrorMessage = "公告类型长度不超过150字符")]
        public string NoticeType { get; set; }

        [Required(ErrorMessage = "公告时间为必填字段")]
        public DateTime NoticeTime { get; set; }

        public string NoticeContent { get; set; }

        [Required(ErrorMessage = "发文部门为必填字段")]
        [MaxLength(128, ErrorMessage = "发文部门长度不超过128字符")]
        public string IssuingDepartment { get; set; }
    }
}


