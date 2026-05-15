namespace EOM.TSHotelManagement.Contract
{
    public class ReadAppointmentNoticeInputDto : ListInputDto
    {
        public string? NoticeId { get; set; }
        public string? NoticeTheme { get; set; }
        public string? NoticeType { get; set; }

        public DateTime? NoticeTime { get; set; }
        public string? NoticeContent { get; set; }
        public string? IssuingDepartment { get; set; }
    }
}


