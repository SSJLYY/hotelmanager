using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateEnergyManagementInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "InformationNumber is required."), MaxLength(128, ErrorMessage = "InformationNumber max length is 128.")]
        public string InformationNumber { get; set; }

        [Required(ErrorMessage = "RoomId is required.")]
        public int? RoomId { get; set; }

        [MaxLength(128, ErrorMessage = "RoomNumber max length is 128.")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "CustomerNumber is required."), MaxLength(128, ErrorMessage = "CustomerNumber max length is 128.")]
        public string CustomerNumber { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "PowerUsage is required.")]
        public decimal PowerUsage { get; set; }

        [Required(ErrorMessage = "WaterUsage is required.")]
        public decimal WaterUsage { get; set; }

        [Required(ErrorMessage = "Recorder is required."), MaxLength(150, ErrorMessage = "Recorder max length is 150.")]
        public string Recorder { get; set; }
    }
}
