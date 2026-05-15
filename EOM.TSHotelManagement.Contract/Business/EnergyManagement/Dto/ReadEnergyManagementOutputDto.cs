using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadEnergyManagementOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public string InformationId { get; set; }
        public int? RoomId { get; set; }
        [UIDisplay("房间号")]
        public string RoomNumber { get; set; }
        public string RoomArea { get; set; }
        public int? RoomFloor { get; set; }
        public string RoomLocator { get; set; }
        [UIDisplay("客户编号")]
        public string CustomerNumber { get; set; }
        [UIDisplay("开始日期")]
        public DateTime StartDate { get; set; }
        [UIDisplay("结束日期")]
        public DateTime EndDate { get; set; }
        [UIDisplay("电力")]
        public decimal PowerUsage { get; set; }
        [UIDisplay("水量")]
        public decimal WaterUsage { get; set; }
        [UIDisplay("记录员")]
        public string Recorder { get; set; }
    }
}

