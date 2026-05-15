using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadCustomerInputDto : ListInputDto
    {
        public string? Name { get; set; }
        public string? CustomerNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdCardNumber { get; set; }
        public int? Gender { get; set; }
        public int? CustomerType { get; set; }
        public DateRangeDto DateRangeDto { get; set; }
    }
}

