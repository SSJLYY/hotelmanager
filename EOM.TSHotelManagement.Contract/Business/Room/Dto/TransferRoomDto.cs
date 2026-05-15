using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class TransferRoomDto : BaseInputDto
    {
        [Required(ErrorMessage = "源房间ID为必填字段")]
        public int? OriginalRoomId { get; set; }
        [MaxLength(128, ErrorMessage = "源房间编号长度不超过128字符")]
        public string OriginalRoomNumber { get; set; }
        public string? OriginalRoomArea { get; set; }
        public int? OriginalRoomFloor { get; set; }

        [Required(ErrorMessage = "目标房间ID为必填字段")]
        public int? TargetRoomId { get; set; }
        [MaxLength(128, ErrorMessage = "目标房间编号长度不超过128字符")]
        public string TargetRoomNumber { get; set; }
        public string? TargetRoomArea { get; set; }
        public int? TargetRoomFloor { get; set; }

        [Required(ErrorMessage = "客户编号为必填字段"), MaxLength(128, ErrorMessage = "客户编号长度不超过128字符")]
        public string CustomerNumber { get; set; }
    }
}
