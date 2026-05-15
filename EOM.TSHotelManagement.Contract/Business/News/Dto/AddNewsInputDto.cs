using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class AddNewsInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "新闻编号为必填字段"), MaxLength(128, ErrorMessage = "新闻编号长度不超过128字符")]
        public string NewId { get; set; }

        [Required(ErrorMessage = "新闻标题为必填字段"), MaxLength(256, ErrorMessage = "新闻标题长度不超过256字符")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "新闻内容为必填字段")]
        public string NewsContent { get; set; }

        [Required(ErrorMessage = "新闻类型为必填字段"), MaxLength(64, ErrorMessage = "新闻类型长度不超过64字符")]
        public string NewsType { get; set; }

        [Required(ErrorMessage = "新闻链接为必填字段"), MaxLength(200, ErrorMessage = "新闻链接长度不超过200字符")]
        public string NewsLink { get; set; }

        [Required(ErrorMessage = "新闻日期为必填字段")]
        public DateTime NewsDate { get; set; }

        [Required(ErrorMessage = "新闻状态为必填字段"), MaxLength(64, ErrorMessage = "新闻状态长度不超过64字符")]
        public string NewsStatus { get; set; }

        [MaxLength(200, ErrorMessage = "新闻图片长度不超过200字符")]
        public string NewsImage { get; set; }
    }
}