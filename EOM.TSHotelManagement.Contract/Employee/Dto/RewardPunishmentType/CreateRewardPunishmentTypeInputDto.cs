using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateRewardPunishmentTypeInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "奖惩类型名称为必填字段")]
        [MaxLength(200, ErrorMessage = "奖惩类型名称长度不超过200字符")]
        public string GBTypeName { get; set; }
    }
}


