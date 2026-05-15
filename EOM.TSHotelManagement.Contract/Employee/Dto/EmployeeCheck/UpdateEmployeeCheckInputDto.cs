using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateEmployeeCheckInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "打卡ID为必填字段")]
        public int CheckId { get; set; }

        [Required(ErrorMessage = "员工工号为必填字段")]
        [MaxLength(128, ErrorMessage = "员工工号长度不超过128字符")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "打卡日期为必填字段")]
        public DateTime CheckDate { get; set; }

        [MaxLength(128, ErrorMessage = "打卡状态长度不超过128字符")]
        public string CheckStatus { get; set; }
    }
}


