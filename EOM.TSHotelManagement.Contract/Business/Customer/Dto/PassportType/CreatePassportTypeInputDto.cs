namespace EOM.TSHotelManagement.Contract
{
    public class CreatePassportTypeInputDto : BaseInputDto
    {
        public int PassportId { get; set; }
        public string PassportName { get; set; }
    }
}

