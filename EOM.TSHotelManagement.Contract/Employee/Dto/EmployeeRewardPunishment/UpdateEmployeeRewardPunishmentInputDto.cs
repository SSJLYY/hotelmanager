using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateEmployeeRewardPunishmentInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "奖惩ID为必填字段")]
        public int RewardPunishmentId { get; set; }

        [Required(ErrorMessage = "员工工号为必填字段")]
        [MaxLength(128, ErrorMessage = "员工工号长度不超过128字符")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "奖惩日期为必填字段")]
        public DateTime RewardPunishmentDate { get; set; }

        [MaxLength(128, ErrorMessage = "奖惩类型长度不超过128字符")]
        public string RewardPunishmentType { get; set; }

        [MaxLength(256, ErrorMessage = "奖惩描述长度不超过256字符")]
        public string RewardPunishmentDescription { get; set; }
    }
}


