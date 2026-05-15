using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class UpdateAdministratorTypeInputDto : BaseInputDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "类型编号为必填字段")]
        [MaxLength(128, ErrorMessage = "类型编号长度不超过128字符")]
        public string TypeId { get; set; }

        [Required(ErrorMessage = "类型名称为必填字段")]
        [MaxLength(256, ErrorMessage = "类型名称长度不超过256字符")]
        public string TypeName { get; set; }
    }
}

