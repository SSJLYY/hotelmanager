namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeRewardPunishmentInputDto : ListInputDto
    {
        public int? RewardPunishmentId { get; set; }
        public string? EmployeeId { get; set; }
    }
}


