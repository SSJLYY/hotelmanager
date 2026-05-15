namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeePhotoInputDto : ListInputDto
    {
        public int? PhotoId { get; set; }
        public string? EmployeeId { get; set; }
    }
}


