namespace EOM.TSHotelManagement.Contract
{
    public class ReadRoomTypeOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public decimal RoomRent { get; set; }
        public decimal RoomDeposit { get; set; }
        public List<RoomTypePricingItemDto>? PricingItems { get; set; }
    }
}

