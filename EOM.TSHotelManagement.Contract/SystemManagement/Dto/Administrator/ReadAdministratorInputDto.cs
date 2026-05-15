namespace EOM.TSHotelManagement.Contract
{
    public class ReadAdministratorInputDto : ListInputDto
    {
        public string? Account { get; set; }
        public string? Password { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? TwoFactorCode { get; set; }

        public int? IsSuperAdmin { get; set; }
    }
}


