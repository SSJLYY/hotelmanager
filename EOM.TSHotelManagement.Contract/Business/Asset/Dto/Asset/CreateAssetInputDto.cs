using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateAssetInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "资产编号为必填字段"), MaxLength(128, ErrorMessage = "资产编号长度不超过128字符")]
        public string AssetNumber { get; set; }

        [Required(ErrorMessage = "资产名称为必填字段"), MaxLength(200, ErrorMessage = "资产名称长度不超过200字符")]
        public string AssetName { get; set; }

        [Required(ErrorMessage = "资产总值为必填字段")]
        public decimal AssetValue { get; set; }

        [Required(ErrorMessage = "部门代码为必填字段"), MaxLength(128, ErrorMessage = "部门代码长度不超过128字符")]
        public string DepartmentCode { get; set; }

        [Required(ErrorMessage = "获取日期为必填字段")]
        public DateTime AcquisitionDate { get; set; }

        [Required(ErrorMessage = "资产来源为必填字段"), MaxLength(500, ErrorMessage = "资产来源长度不超过500字符")]
        public string AssetSource { get; set; }

        [Required(ErrorMessage = "经办员工为必填字段"), MaxLength(128, ErrorMessage = "经办员工长度不超过128字符")]
        public string AcquiredByEmployeeId { get; set; }
    }
}

