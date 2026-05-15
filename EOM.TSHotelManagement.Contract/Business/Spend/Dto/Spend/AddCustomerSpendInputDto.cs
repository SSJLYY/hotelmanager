using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class AddCustomerSpendInputDto
    {
        [Required(ErrorMessage = "SpendNumber is required.")]
        public string SpendNumber { get; set; }

        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "RoomId is required.")]
        public int? RoomId { get; set; }

        [Required(ErrorMessage = "ProductNumber is required.")]
        public string ProductNumber { get; set; }

        [Required(ErrorMessage = "ProductName is required.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "ConsumptionQuantity is required.")]
        public int ConsumptionQuantity { get; set; }

        [Required(ErrorMessage = "ProductPrice is required.")]
        public decimal ProductPrice { get; set; }

        public string SoftwareVersion { get; set; }
    }
}
