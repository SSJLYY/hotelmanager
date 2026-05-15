namespace EOM.TSHotelManagement.Contract
{
    public class ReadVipLevelRuleInputDto : ListInputDto
    {
        public int? RuleId { get; set; }
        public string? RuleSerialNumber { get; set; }
        public string? RuleName { get; set; }
        public int? VipLevelId { get; set; }
    }
}



