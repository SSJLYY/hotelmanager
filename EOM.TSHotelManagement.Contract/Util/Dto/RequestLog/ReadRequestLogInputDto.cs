using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadRequestLogInputDto : ListInputDto
    {
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? UserName { get; set; }
        public DateRangeDto DateRangeDto { get; set; }
    }
}