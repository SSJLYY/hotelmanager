namespace EOM.TSHotelManagement.Contract
{
    public class CurrentProfileOutputDto
    {
        public string LoginType { get; set; }

        public string UserNumber { get; set; }

        public string Account { get; set; }

        public string DisplayName { get; set; }

        public string PhotoUrl { get; set; }

        public object? Profile { get; set; }
    }

    public class CurrentProfileEmployeeDto
    {
        public string EmployeeId { get; set; }

        public string Name { get; set; }

        public string DepartmentName { get; set; }

        public string PositionName { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public string HireDate { get; set; }

        public string DateOfBirth { get; set; }

        public string PhotoUrl { get; set; }
    }

    public class CurrentProfileAdminDto
    {
        public string Number { get; set; }

        public string Account { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public string Type { get; set; }

        public int IsSuperAdmin { get; set; }

        public string PhotoUrl { get; set; }
    }
}
