using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class CreateDepartmentInputDto : BaseInputDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "部门编号为必填字段")]
        [MaxLength(128, ErrorMessage = "部门编号长度不超过128字符")]
        public string DepartmentNumber { get; set; }

        [Required(ErrorMessage = "部门名称为必填字段")]
        [MaxLength(256, ErrorMessage = "部门名称长度不超过256字符")]
        public string DepartmentName { get; set; }

        [MaxLength(500, ErrorMessage = "部门描述长度不超过500字符")]
        public string DepartmentDescription { get; set; }

        public DateOnly DepartmentCreationDate { get; set; }

        [Required(ErrorMessage = "部门主管为必填字段")]
        [MaxLength(128, ErrorMessage = "部门主管长度不超过128字符")]
        public string DepartmentLeader { get; set; }

        [MaxLength(200, ErrorMessage = "部门主管姓名长度不超过200字符")]
        public string LeaderName { get; set; }

        [MaxLength(128, ErrorMessage = "上级部门编号长度不超过128字符")]
        public string ParentDepartmentNumber { get; set; }

        [MaxLength(256, ErrorMessage = "上级部门名称长度不超过256字符")]
        public string ParentDepartmentName { get; set; }
    }
}


