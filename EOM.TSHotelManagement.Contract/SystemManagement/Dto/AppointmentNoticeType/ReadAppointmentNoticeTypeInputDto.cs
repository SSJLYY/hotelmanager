namespace EOM.TSHotelManagement.Contract
{
    public class ReadAppointmentNoticeTypeInputDto : ListInputDto
    {
        public string? NoticeTypeNumber { get; set; }
        public string? NoticeTypeName { get; set; } = string.Empty;
    }
}
