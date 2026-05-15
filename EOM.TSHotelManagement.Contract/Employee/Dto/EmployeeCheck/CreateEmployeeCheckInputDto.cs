using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateEmployeeCheckInputDto : BaseInputDto
    {
        /// <summary>
        /// 员工工号 (Employee ID)
        /// </summary>
        [Required(ErrorMessage = "员工工号为必填字段")]
        [MaxLength(128, ErrorMessage = "员工工号长度不超过128字符")]
        public string EmployeeId { get; set; }
    }
}


