namespace EOM.TSHotelManagement.Contract
{
    public class ReadAssetOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public string AssetNumber { get; set; }
        public string AssetName { get; set; }

        public decimal AssetValue { get; set; }

        public string AssetValueFormatted { get; set; }
        public string DepartmentCode { get; set; }

        public string DepartmentName { get; set; }

        public DateTime AcquisitionDate { get; set; }
        public string AssetSource { get; set; }
        public string AcquiredByEmployeeId { get; set; }

        public string AcquiredByName { get; set; }
    }
}

