namespace EOM.TSHotelManagement.Contract
{
    public class ReadPositionInputDto : ListInputDto
    {
        public int? PositionId { get; set; }
        public string? PositionName { get; set; }
    }
}



