namespace EOM.TSHotelManagement.Service
{
    internal sealed class RoomPricingEvaluation
    {
        public string SelectedPricingCode { get; init; }
        public string SelectedPricingName { get; init; }
        public string EffectivePricingCode { get; init; }
        public string EffectivePricingName { get; init; }
        public decimal EffectiveRoomRent { get; init; }
        public decimal EffectiveRoomDeposit { get; init; }
        public int? PricingStayHours { get; init; }
        public bool IsPricingTimedOut { get; init; }
    }
}
