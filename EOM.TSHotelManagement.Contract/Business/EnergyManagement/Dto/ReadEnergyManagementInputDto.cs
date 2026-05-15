using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadEnergyManagementInputDto : ListInputDto
    {
        public string? CustomerNumber { get; set; }
        public int? RoomId { get; set; }
        public string? RoomNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
