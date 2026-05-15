using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadSpendOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public int? RoomId { get; set; }

        [UIDisplay("消费编号", false, false)]
        public string SpendNumber { get; set; }

        [UIDisplay("房间号")]
        public string RoomNumber { get; set; }

        public string RoomArea { get; set; }

        public int? RoomFloor { get; set; }

        public string RoomLocator { get; set; }

        [UIDisplay("客户编号")]
        public string CustomerNumber { get; set; }

        [UIDisplay("商品编号", false, false)]
        public string ProductNumber { get; set; }

        [UIDisplay("商品名称")]
        public string ProductName { get; set; }

        [UIDisplay("消费数量", true)]
        public int ConsumptionQuantity { get; set; }

        [UIDisplay("商品单价")]
        public decimal ProductPrice { get; set; }

        public string ProductPriceFormatted { get; set; }

        [UIDisplay("消费金额")]
        public decimal ConsumptionAmount { get; set; }

        public string ConsumptionAmountFormatted { get; set; }

        [UIDisplay("消费时间")]
        public DateTime ConsumptionTime { get; set; }
        public string SettlementStatus { get; set; }

        [UIDisplay("消费类型", false, false)]
        public string ConsumptionType { get; set; }

        [UIDisplay("消费类型", false, true)]
        public string ConsumptionTypeDescription { get; set; }

        [UIDisplay("结算状态")]
        public string SettlementStatusDescription { get; set; }
    }
}

