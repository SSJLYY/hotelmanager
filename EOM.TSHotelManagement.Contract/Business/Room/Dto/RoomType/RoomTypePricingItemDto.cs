namespace EOM.TSHotelManagement.Contract
{
    public class RoomTypePricingItemDto
    {
        public string PricingCode { get; set; }
        public string PricingName { get; set; }
        public decimal RoomRent { get; set; }
        public decimal RoomDeposit { get; set; }
        public int? StayHours { get; set; }
        public int Sort { get; set; }
        public bool IsDefault { get; set; }
    }
}
