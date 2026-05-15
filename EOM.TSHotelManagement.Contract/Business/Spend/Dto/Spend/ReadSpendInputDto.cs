using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadSpendInputDto : ListInputDto
    {
        public string? SpendNumber { get; set; }

        public int? RoomId { get; set; }

        public string? RoomNumber { get; set; }

        public string? CustomerNumber { get; set; }

        public string? ProductName { get; set; }

        public int? ConsumptionQuantity { get; set; }

        public decimal? ProductPrice { get; set; }

        public decimal? ConsumptionAmount { get; set; }

        public string? SettlementStatus { get; set; }

        public DateRangeDto DateRangeDto { get; set; }
    }
}
