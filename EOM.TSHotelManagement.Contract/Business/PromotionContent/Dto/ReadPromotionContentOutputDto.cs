namespace EOM.TSHotelManagement.Contract
{
    public class ReadPromotionContentOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public string PromotionContentNumber { get; set; }
        public string PromotionContentMessage { get; set; }
    }
}

