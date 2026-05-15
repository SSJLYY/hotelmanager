namespace EOM.TSHotelManagement.Contract
{
    public class ReadRoomOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public string RoomNumber { get; set; }
        public string RoomArea { get; set; }
        public int? RoomFloor { get; set; }
        public string RoomLocator { get; set; }
        public string RoomName { get; set; }
        public int RoomTypeId { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime? LastCheckInTime { get; set; }
        public DateTime? LastCheckOutTime { get; set; }
        public int RoomStateId { get; set; }
        public string RoomState { get; set; }
        public decimal RoomRent { get; set; }
        public decimal RoomDeposit { get; set; }
        public decimal StandardRoomRent { get; set; }
        public decimal StandardRoomDeposit { get; set; }
        public decimal AppliedRoomRent { get; set; }
        public decimal AppliedRoomDeposit { get; set; }
        public decimal EffectiveRoomRent { get; set; }
        public decimal EffectiveRoomDeposit { get; set; }
        public string EffectivePricingCode { get; set; }
        public string EffectivePricingName { get; set; }
        public string PricingCode { get; set; }
        public string PricingName { get; set; }
        public int? PricingStayHours { get; set; }
        public bool IsPricingTimedOut { get; set; }
        public string RoomLocation { get; set; }

        public int StayDays { get; set; }
        public int Vacant { get; set; }
        public int Occupied { get; set; }
        public int Maintenance { get; set; }
        public int Dirty { get; set; }
        public int Reserved { get; set; }
    }
}
