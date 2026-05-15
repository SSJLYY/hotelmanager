namespace EOM.TSHotelManagement.Contract
{
    public class ReadVipLevelRuleOutputDto : BaseOutputDto
    {
        public string RuleSerialNumber { get; set; }
        public string RuleName { get; set; }

        public decimal RuleValue { get; set; }
        public int VipLevelId { get; set; }
        public string VipLevelName { get; set; }
    }
}



