using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateEmployeePhotoInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "员工工号为必填字段")]
        [MaxLength(128, ErrorMessage = "员工工号长度不超过128字符")]
        public string EmployeeId { get; set; }

        [MaxLength(256, ErrorMessage = "照片路径长度不超过256字符")]
        public string PhotoUrl { get; set; }
    }
}


