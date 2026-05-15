using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateReserInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "ReservationId is required."), MaxLength(128, ErrorMessage = "ReservationId max length is 128.")]
        public string ReservationId { get; set; }

        [Required(ErrorMessage = "CustomerName is required."), MaxLength(200, ErrorMessage = "CustomerName max length is 200.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "ReservationPhoneNumber is required."), MaxLength(256, ErrorMessage = "ReservationPhoneNumber max length is 256.")]
        public string ReservationPhoneNumber { get; set; }

        [Required(ErrorMessage = "RoomId is required.")]
        public int? RoomId { get; set; }

        [MaxLength(128, ErrorMessage = "ReservationRoomNumber max length is 128.")]
        public string ReservationRoomNumber { get; set; }

        [Required(ErrorMessage = "ReservationChannel is required."), MaxLength(50, ErrorMessage = "ReservationChannel max length is 50.")]
        public string ReservationChannel { get; set; }

        [Required(ErrorMessage = "ReservationStartDate is required.")]
        public DateTime ReservationStartDate { get; set; }

        [Required(ErrorMessage = "ReservationEndDate is required.")]
        public DateTime ReservationEndDate { get; set; }
    }
}
