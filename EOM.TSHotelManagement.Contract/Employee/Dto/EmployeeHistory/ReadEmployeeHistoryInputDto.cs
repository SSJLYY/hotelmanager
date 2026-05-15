namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeHistoryInputDto : ListInputDto
    {
        public int? HistoryId { get; set; }
        public string? EmployeeId { get; set; }
    }
}


