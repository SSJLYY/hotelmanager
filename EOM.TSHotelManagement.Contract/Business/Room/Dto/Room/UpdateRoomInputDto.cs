namespace EOM.TSHotelManagement.Contract
{
    public class UpdateRoomInputDto : BaseInputDto
    {
        public int? Id { get; set; }
        public string RoomNumber { get; set; }
        public string? RoomArea { get; set; }
        public int? RoomFloor { get; set; }
        public int RoomTypeId { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? LastCheckInTime { get; set; }
        public DateTime? LastCheckOutTime { get; set; }
        public int RoomStateId { get; set; }
        public decimal RoomRent { get; set; }
        public decimal RoomDeposit { get; set; }
        public string RoomLocation { get; set; }
        public string? PricingCode { get; set; }
    }
}
