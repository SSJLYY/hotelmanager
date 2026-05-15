using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateSpendInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "SpendNumber is required.")]
        public string SpendNumber { get; set; }

        [Required(ErrorMessage = "RoomId is required.")]
        public int? RoomId { get; set; }

        public string RoomNumber { get; set; }

        public string OriginalRoomNumber { get; set; }

        public string CustomerNumber { get; set; }

        public string ProductName { get; set; }

        [Required(ErrorMessage = "ConsumptionQuantity is required.")]
        public int ConsumptionQuantity { get; set; }

        [Required(ErrorMessage = "ProductPrice is required.")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "ConsumptionAmount is required.")]
        public decimal ConsumptionAmount { get; set; }

        [Required(ErrorMessage = "ConsumptionTime is required.")]
        public DateTime ConsumptionTime { get; set; }

        public string SettlementStatus { get; set; }

        public string ConsumptionType { get; set; }
    }
}
