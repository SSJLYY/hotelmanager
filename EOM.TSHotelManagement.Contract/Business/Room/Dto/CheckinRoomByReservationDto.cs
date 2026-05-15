using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CheckinRoomByReservationDto : BaseInputDto
    {
        [Required(ErrorMessage = "客户编号为必填字段"), MaxLength(128, ErrorMessage = "客户编号长度不超过128字符")]
        public string CustomerNumber { get; set; }

        [Required(ErrorMessage = "客户名称为必填字段"), MaxLength(200, ErrorMessage = "客户名称长度不超过200字符")]
        public string CustomerName { get; set; }

        public int? CustomerGender { get; set; }

        [Required(ErrorMessage = "护照ID为必填字段")]
        public int PassportId { get; set; }

        [Required(ErrorMessage = "客户电话为必填字段"), MaxLength(256, ErrorMessage = "客户电话长度不超过256字符")]
        public string CustomerPhoneNumber { get; set; }

        [Required(ErrorMessage = "出生日期为必填字段")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "身份证号为必填字段"), MaxLength(128, ErrorMessage = "身份证号长度不超过128字符")]
        public string IdCardNumber { get; set; }

        [MaxLength(500, ErrorMessage = "客户地址长度不超过500字符")]
        public string CustomerAddress { get; set; }

        [Required(ErrorMessage = "客户类型为必填字段")]
        public int CustomerType { get; set; }

        [Required(ErrorMessage = "房间ID为必填字段")]
        public int? RoomId { get; set; }

        [MaxLength(128, ErrorMessage = "房间编号长度不超过128字符")]
        public string RoomNumber { get; set; }
        public string? RoomArea { get; set; }
        public int? RoomFloor { get; set; }
        public string? PricingCode { get; set; }

        [Required(ErrorMessage = "预约编号为必填字段"), MaxLength(128, ErrorMessage = "预约编号长度不超过128字符")]
        public string ReservationId { get; set; }
    }
}
