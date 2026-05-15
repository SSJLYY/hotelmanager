namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeRewardPunishmentOutputDto : BaseOutputDto
    {
        public string EmployeeId { get; set; }

        public DateTime RewardPunishmentTime { get; set; }

        public int RewardPunishmentType { get; set; }
        public string RewardPunishmentTypeName { get; set; }
        public string RewardPunishmentInformation { get; set; }
        public string RewardPunishmentOperator { get; set; }
        public string OperatorName { get; set; }
    }
}


