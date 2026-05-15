namespace EOM.TSHotelManagement.Contract
{
    public class CreateCardCodeInputDto : BaseInputDto
    {
        public long Id { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string AreaCode { get; set; }
    }
}



