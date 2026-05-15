using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreatePromotionContentInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "宣传ID为必填字段"), MaxLength(128, ErrorMessage = "宣传ID长度不超过128字符")]
        public string PromotionContentNumber { get; set; }

        [Required(ErrorMessage = "宣传内容为必填字段"), MaxLength(2000, ErrorMessage = "宣传内容长度不超过2000字符")]
        public string PromotionContentMessage { get; set; }
    }
}

