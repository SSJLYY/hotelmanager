namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeHistoryOutputDto : BaseOutputDto
    {
        public string EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
    }
}


