using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateOperationLogInputDto : BaseInputDto
    {
        public int OperationId { get; set; }
        public DateTime OperationTime { get; set; }
        public string LogContent { get; set; }
        public string OperationAccount { get; set; }
        public LogLevel LogLevel { get; set; }
        public string SoftwareVersion { get; set; }
        public string LoginIpAddress { get; set; }
    }
}

