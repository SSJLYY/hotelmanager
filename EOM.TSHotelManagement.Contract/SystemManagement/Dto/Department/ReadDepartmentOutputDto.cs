namespace EOM.TSHotelManagement.Contract
{
    public class ReadDepartmentOutputDto : BaseOutputDto
    {
        public string DepartmentNumber { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }

        public DateOnly DepartmentCreationDate { get; set; }
        public string DepartmentLeader { get; set; }
        public string LeaderName { get; set; }
        public string ParentDepartmentNumber { get; set; }
        public string ParentDepartmentName { get; set; }
    }
}


