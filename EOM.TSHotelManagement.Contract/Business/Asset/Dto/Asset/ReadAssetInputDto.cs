using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadAssetInputDto : ListInputDto
    {
        public string? AssetNumber { get; set; }
        public string? AssetName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? AcquiredByEmployeeId { get; set; }

        public DateRangeDto DateRangeDto { get; set; }
    }
}

