namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeePhotoOutputDto : BaseOutputDto
    {
        public int PhotoId { get; set; }
        public string EmployeeId { get; set; }
        public string PhotoPath { get; set; }
    }
}


