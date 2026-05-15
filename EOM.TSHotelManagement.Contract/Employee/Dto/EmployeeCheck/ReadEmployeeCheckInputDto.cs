namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeCheckInputDto : ListInputDto
    {
        public int? CheckId { get; set; }
        public string? EmployeeId { get; set; }
    }
}


