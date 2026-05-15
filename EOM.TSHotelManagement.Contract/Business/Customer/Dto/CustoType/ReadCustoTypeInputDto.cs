namespace EOM.TSHotelManagement.Contract
{
    public class ReadCustoTypeInputDto : ListInputDto
    {
        public int? CustomerType { get; set; }
        public string? CustomerTypeName { get; set; }
    }
}
