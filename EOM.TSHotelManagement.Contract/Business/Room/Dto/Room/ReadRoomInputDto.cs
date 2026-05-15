namespace EOM.TSHotelManagement.Contract
{
    public class ReadRoomInputDto : ListInputDto
    {
        public int? Id { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomArea { get; set; }
        public int? RoomFloor { get; set; }
        public int? RoomTypeId { get; set; }
        public int? RoomStateId { get; set; }
        public string? RoomName { get; set; }
        public DateTime? LastCheckInTime { get; set; }
        public string? CustomerNumber { get; set; }
        public string? PricingCode { get; set; }
    }
}
