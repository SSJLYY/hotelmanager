namespace EOM.TSHotelManagement.Contract
{
    public class ReadRoleInputDto : ListInputDto
    {
        public string? RoleNumber { get; set; } = null!;
        public string? RoleName { get; set; } = null!;
        public string? RoleDescription { get; set; }
    }
}


