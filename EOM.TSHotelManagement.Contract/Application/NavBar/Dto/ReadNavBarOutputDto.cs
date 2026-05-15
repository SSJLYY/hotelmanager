namespace EOM.TSHotelManagement.Contract
{
    public class ReadNavBarOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }

        public int NavigationBarId { get; set; }
        public string NavigationBarName { get; set; }

        public int NavigationBarOrder { get; set; }
        public string NavigationBarImage { get; set; }
        public string NavigationBarEvent { get; set; }

        public int MarginLeft { get; set; }
    }
}

