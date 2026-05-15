namespace EOM.TSHotelManagement.Contract
{
    public class ReadRoomTypeInputDto : ListInputDto
    {
        public int? Id { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomArea { get; set; }
        public int? RoomFloor { get; set; }
        public int? RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public decimal? RoomRent { get; set; }
        public decimal? RoomDeposit { get; set; }
    }
}

