using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UploadAvatarInputDto
    {
        [MaxLength(32, ErrorMessage = "账号类型长度不能超过32字符")]
        public string? LoginType { get; set; }
    }
}
