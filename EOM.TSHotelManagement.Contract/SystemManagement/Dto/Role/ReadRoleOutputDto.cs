namespace EOM.TSHotelManagement.Contract
{
    public class ReadRoleOutputDto : BaseOutputDto
    {
        public string RoleNumber { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string? RoleDescription { get; set; }
    }
}


