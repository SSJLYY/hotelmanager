namespace EOM.TSHotelManagement.Contract
{
    public class ReadAppointmentNoticeTypeOutputDto : BaseOutputDto
    {
        public string NoticeTypeNumber { get; set; }
        public string NoticeTypeName { get; set; }

        public int? IsDelete { get; set; } = 0;
        public string DataInsUsr { get; set; }
        public DateTime? DataInsDate { get; set; }
        public string DataChgUsr { get; set; }
        public DateTime? DataChgDate { get; set; }
    }
}
