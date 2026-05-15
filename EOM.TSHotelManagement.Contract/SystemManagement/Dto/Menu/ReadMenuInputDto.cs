namespace EOM.TSHotelManagement.Contract
{
    public class ReadMenuInputDto : ListInputDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public bool SearchParent { get; set; } = false;
    }
}

