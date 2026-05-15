
using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadCustomerOutputDto : BaseOutputDto
    {
        [UIDisplay("ID", true, false)]
        public int? Id { get; set; }

        [UIDisplay("客户编号")]
        public string CustomerNumber { get; set; }

        [UIDisplay("客户姓名")]
        public string Name { get; set; }

        [UIDisplay("性别", true, false)]
        public int? Gender { get; set; }

        [UIDisplay("证件类型", true, false)]
        public int IdCardType { get; set; }

        [UIDisplay("性别", false, true)]
        public string GenderName { get; set; }

        [UIDisplay("联系方式")]
        public string PhoneNumber { get; set; }

        [UIDisplay("出生日期")]
        public DateTime DateOfBirth { get; set; }

        [UIDisplay("客户类型", true, false)]
        public int CustomerType { get; set; }

        [UIDisplay("客户类型", false, true)]
        public string CustomerTypeName { get; set; }

        [UIDisplay("证件类型", false, true)]
        public string PassportName { get; set; }

        [UIDisplay("证件号码")]
        public string IdCardNumber { get; set; }

        [UIDisplay("客户地址")]
        public string Address { get; set; }
    }
}

