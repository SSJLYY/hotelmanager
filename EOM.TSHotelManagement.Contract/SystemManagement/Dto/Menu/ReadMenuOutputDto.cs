namespace EOM.TSHotelManagement.Contract
{
    public class ReadMenuOutputDto : BaseOutputDto
    {
        public string Key { get; set; }
        public string Title { get; set; }

        public string Path { get; set; }
        public int? Parent { get; set; }
        public string Icon { get; set; }
    }
}


