namespace EOM.TSHotelManagement.Contract
{
    public class ReadRewardPunishmentTypeInputDto : ListInputDto
    {
        public string? RewardPunishmentTypeId { get; set; }
        public string? RewardPunishmentTypeName { get; set; } = string.Empty;
    }
}


