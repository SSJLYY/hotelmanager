using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateVipLevelRuleInputDto : BaseInputDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "会员规则流水号为必填字段")]
        [MaxLength(128, ErrorMessage = "会员规则流水号长度不超过128字符")]
        public string RuleSerialNumber { get; set; }

        [Required(ErrorMessage = "会员规则名称为必填字段")]
        [MaxLength(200, ErrorMessage = "会员规则名称长度不超过200字符")]
        public string RuleName { get; set; }

        [Required(ErrorMessage = "会员规则值为必填字段")]
        public decimal RuleValue { get; set; }

        [Required(ErrorMessage = "VIP等级ID为必填字段")]
        public int VipLevelId { get; set; }

        public string VipLevelName { get; set; }
    }
}



