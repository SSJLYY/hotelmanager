using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateEmployeeHistoryInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "履历编号为必填字段")]
        public string HistoryNumber { get; set; }

        [Required(ErrorMessage = "员工工号为必填字段")]
        [MaxLength(128, ErrorMessage = "员工工号长度不超过128字符")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// 开始时间 (Start Date)
        /// </summary>
        [Required(ErrorMessage = "开始时间为必填字段")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// 结束时间 (End Date)
        /// </summary>
        [Required(ErrorMessage = "结束时间为必填字段")]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// 职位 (Position)
        /// </summary>
        [Required(ErrorMessage = "职位为必填字段")]
        public string Position { get; set; }

        /// <summary>
        /// 公司 (Company)
        /// </summary>
        [Required(ErrorMessage = "公司为必填字段")]
        public string Company { get; set; }
    }
}


