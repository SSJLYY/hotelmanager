namespace EOM.TSHotelManagement.Contract
{
    public class ReadRoomPricingOutputDto
    {
        public int? RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomLocator { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string CurrentPricingCode { get; set; }
        public string CurrentPricingName { get; set; }
        public int? PricingStayHours { get; set; }
        public bool IsPricingTimedOut { get; set; }
        public string EffectivePricingCode { get; set; }
        public string EffectivePricingName { get; set; }
        public DateTime? LastCheckInTime { get; set; }
        public decimal EffectiveRoomRent { get; set; }
        public decimal EffectiveRoomDeposit { get; set; }
        public List<RoomTypePricingItemDto> PricingItems { get; set; }
    }
}
