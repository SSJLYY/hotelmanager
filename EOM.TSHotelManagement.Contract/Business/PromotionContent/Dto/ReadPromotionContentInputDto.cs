namespace EOM.TSHotelManagement.Contract
{
    public class ReadPromotionContentInputDto : ListInputDto
    {
        public int? PromotionId { get; set; }
        public string? PromotionContentNumber { get; set; } = string.Empty;
        public string? PromotionContentMessage { get; set; } = string.Empty;
    }
}

