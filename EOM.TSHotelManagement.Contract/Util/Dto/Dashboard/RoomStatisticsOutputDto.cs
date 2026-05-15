using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EOM.TSHotelManagement.Contract
{
    public class RoomStatisticsOutputDto
    {
        [JsonPropertyName("status")]
        public TempRoomStatus Status { get; set; }

        [JsonPropertyName("types")]
        public List<TempRoomType> Types { get; set; }

        [JsonPropertyName("reservationAlerts")]
        public List<TempReservationAlert> ReservationAlerts { get; set; }
    }
    public class TempRoomStatus
    {
        [JsonPropertyName("空房")]
        public int Vacant { get; set; }

        [JsonPropertyName("已住")]
        public int Occupied { get; set; }

        [JsonPropertyName("维修")]
        public int Maintenance { get; set; }

        [JsonPropertyName("脏房")]
        public int Dirty { get; set; }

        [JsonPropertyName("预约")]
        public int Reserved { get; set; }
    }

    public class TempRoomType
    {
        [JsonPropertyName("name")]
        [MaxLength(100, ErrorMessage = "房间类型名称长度不超过100字符")]
        public string Name { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }
    }

    public class TempReservationAlert
    {
        [JsonPropertyName("roomType")]
        [MaxLength(100, ErrorMessage = "房间类型长度不超过100字符")]
        public string RoomType { get; set; }
        [JsonPropertyName("guestName")]
        [MaxLength(256, ErrorMessage = "客人姓名长度不超过256字符")]
        public string GuestName { get; set; }
        [JsonPropertyName("guestPhoneNo")]
        [MaxLength(256, ErrorMessage = "客人电话长度不超过256字符")]
        public string GuestPhoneNo { get; set; }
        [JsonPropertyName("endDate")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}