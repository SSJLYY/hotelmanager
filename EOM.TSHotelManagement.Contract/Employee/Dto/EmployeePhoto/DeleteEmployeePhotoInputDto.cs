using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    public class DeleteEmployeePhotoInputDto
    {
        [Required(ErrorMessage = "Employee Id is required")]
        public int EmployeeId { get; set; }
    }
}


