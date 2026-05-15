namespace EOM.TSHotelManagement.Contract
{
    public class ReadPassportTypeInputDto : ListInputDto
    {
        public int? PassportId { get; set; }
        public string? PassportName { get; set; }
    }
}
