using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateSupervisionStatisticsInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "统计编号为必填字段")]
        [MaxLength(128, ErrorMessage = "统计编号长度不超过128字符")]
        public string StatisticsNumber { get; set; }

        [MaxLength(128, ErrorMessage = "监督部门长度不超过128字符")]
        public string SupervisingDepartment { get; set; }

        [MaxLength(200, ErrorMessage = "监督部门名称长度不超过200字符")]
        public string SupervisingDepartmentName { get; set; }

        public string SupervisionProgress { get; set; }
        public string SupervisionLoss { get; set; }
        public int SupervisionScore { get; set; }

        [MaxLength(128, ErrorMessage = "监督统计事长度不超过128字符")]
        public string SupervisionStatistician { get; set; }

        public string SupervisionAdvice { get; set; }

        [Required(ErrorMessage = "统计时间为必填字段")]
        public DateTime? SupervisionTime { get; set; }
    }
}



