namespace EOM.TSHotelManagement.Contract
{
    public class ReadSupervisionStatisticsOutputDto : BaseOutputDto
    {
        public string StatisticsNumber { get; set; }
        public string SupervisingDepartment { get; set; }
        public string SupervisingDepartmentName { get; set; }

        public string SupervisionProgress { get; set; }
        public string SupervisionLoss { get; set; }
        public int SupervisionScore { get; set; }
        public string SupervisionStatistician { get; set; }

        public string SupervisionAdvice { get; set; }

        public DateTime SupervisionTime { get; set; }
    }
}



