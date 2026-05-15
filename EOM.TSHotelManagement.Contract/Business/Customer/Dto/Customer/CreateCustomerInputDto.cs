using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateCustomerInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "客户编号为必填字段"), MaxLength(128, ErrorMessage = "客户编号长度不超过128字符")]
        public string CustomerNumber { get; set; }

        [Required(ErrorMessage = "客户名称为必填字段"), MaxLength(250, ErrorMessage = "客户名称长度不超过250字符")]
        public string Name { get; set; }

        [Required(ErrorMessage = "客户性别为必填字段")]
        public int? Gender { get; set; }

        [Required(ErrorMessage = "证件类型为必填字段")]
        public int IdCardType { get; set; }

        [Required(ErrorMessage = "客户电话为必填字段"), MaxLength(256, ErrorMessage = "客户电话长度不超过256字符")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "出生日期为必填字段")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "证件号码为必填字段"), MaxLength(256, ErrorMessage = "证件号码长度不超过256字符")]
        public string IdCardNumber { get; set; }

        [MaxLength(256, ErrorMessage = "客户地址长度不超过256字符")]
        public string Address { get; set; }

        [Required(ErrorMessage = "客户类型为必填字段")]
        public int CustomerType { get; set; }
    }
}

