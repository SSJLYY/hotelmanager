using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateRewardPunishmentTypeInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "奖惩类型编号为必填字段")]
        [MaxLength(128, ErrorMessage = "奖惩类型编号长度不超过128字符")]
        public string RewardPunishmentTypeId { get; set; }

        [Required(ErrorMessage = "奖惩类型名称为必填字段")]
        [MaxLength(200, ErrorMessage = "奖惩类型名称长度不超过200字符")]
        public string RewardPunishmentTypeName { get; set; }
    }
}


