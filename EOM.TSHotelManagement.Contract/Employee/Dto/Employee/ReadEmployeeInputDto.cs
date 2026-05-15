using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeInputDto : ListInputDto
    {
        public string? EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        public string? Ethnicity { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
        public string? Address { get; set; }
        public string? Position { get; set; }
        public string? IdCardNumber { get; set; }

        public string? PoliticalAffiliation { get; set; }
        public string? EducationLevel { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }

        public bool? IsEnable { get; set; }

        public DateRangeDto DateRangeDto { get; set; }
    }
}


