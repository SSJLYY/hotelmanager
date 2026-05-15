namespace EOM.TSHotelManagement.Contract
{
    public class ReadPassportTypeOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public int PassportId { get; set; }
        public string PassportName { get; set; }
    }
}
