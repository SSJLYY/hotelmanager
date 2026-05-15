namespace EOM.TSHotelManagement.Contract
{
    public class ReadReserInputDto : ListInputDto
    {
        public int? RoomId { get; set; }
        public string? ReservationId { get; set; }
        public string? CustomerName { get; set; }
        public string? ReservationPhoneNumber { get; set; }
        public string? ReservationRoomNumber { get; set; }
        public string? ReservationChannel { get; set; }

        public DateTime? ReservationStartDateStart { get; set; }

        public DateTime? ReservationStartDateEnd { get; set; }
    }
}

