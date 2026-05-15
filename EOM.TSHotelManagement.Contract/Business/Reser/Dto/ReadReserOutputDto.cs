using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadReserOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public int? RoomId { get; set; }

        [UIDisplay("预约编号")]
        public string ReservationId { get; set; }

        [UIDisplay("客户姓名")]
        public string CustomerName { get; set; }

        [UIDisplay("联系方式")]
        public string ReservationPhoneNumber { get; set; }

        [UIDisplay("预定房号")]
        public string ReservationRoomNumber { get; set; }

        public string RoomArea { get; set; }

        public int? RoomFloor { get; set; }

        public string RoomLocator { get; set; }

        [UIDisplay("预约渠道")]
        public string ReservationChannel { get; set; }

        public string ReservationChannelDescription { get; set; }

        [UIDisplay("预约开始时")]
        public DateTime ReservationStartDate { get; set; }

        [UIDisplay("预约结束时")]
        public DateTime ReservationEndDate { get; set; }
    }
}

