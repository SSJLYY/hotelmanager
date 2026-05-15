namespace EOM.TSHotelManagement.Contract
{
    public class UpdateRoomStateInputDto : BaseInputDto
    {
        public int RoomStateId { get; set; }
        public string RoomStateName { get; set; }
    }
}


