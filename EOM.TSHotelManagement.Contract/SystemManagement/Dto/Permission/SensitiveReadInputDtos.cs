using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// Request body for reading data by user number.
    /// </summary>
    public class ReadByUserNumberInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "UserNumber is required.")]
        [MaxLength(128, ErrorMessage = "UserNumber cannot exceed 128 characters.")]
        public string UserNumber { get; set; } = null!;
    }

    /// <summary>
    /// Request body for reading data by role number.
    /// </summary>
    public class ReadByRoleNumberInputDto : BaseInputDto
    {
        [Required(ErrorMessage = "RoleNumber is required.")]
        [MaxLength(128, ErrorMessage = "RoleNumber cannot exceed 128 characters.")]
        public string RoleNumber { get; set; } = null!;
    }
}
