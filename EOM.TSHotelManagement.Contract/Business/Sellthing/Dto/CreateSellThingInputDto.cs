using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateSellThingInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "商品编号为必填字段"), MaxLength(128, ErrorMessage = "商品编号长度不超过128字符")]
        public string ProductNumber { get; set; }

        [Required(ErrorMessage = "商品名称为必填字段"), MaxLength(500, ErrorMessage = "商品名称长度不超过500字符")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "商品价格为必填字段")]
        public decimal ProductPrice { get; set; }

        [MaxLength(1000, ErrorMessage = "规格型号长度不超过1000字符")]
        public string Specification { get; set; }

        [Required(ErrorMessage = "库存数量为必填字段")]
        public int Stock { get; set; }
    }
}


