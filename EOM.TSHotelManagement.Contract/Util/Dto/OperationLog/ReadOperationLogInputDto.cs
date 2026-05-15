
using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadOperationLogInputDto : ListInputDto
    {
        public string? OperationId { get; set; }
        public int? LogLevel { get; set; }
        public string? LoginIpAddress { get; set; }
        public string? OperationAccount { get; set; }
        public DateRangeDto DateRangeDto { get; set; }
    }
}
