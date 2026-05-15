namespace EOM.TSHotelManagement.Contract
{
    public class ReadCardCodeOutputDto : BaseOutputDto
    {

        public string Province { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string AreaCode { get; set; }
    }
}


