using SqlSugar;

namespace EOM.TSHotelManagement.Contract
{
    public class ReadEmployeeCheckOutputDto : BaseOutputDto
    {
        public string EmployeeId { get; set; }

        public DateTime CheckTime { get; set; }
        public int CheckStatus { get; set; }
        public string CheckMethod { get; set; }
        public string CheckMethodDescription { get; set; }

        public bool MorningChecked { get; set; }

        public bool EveningChecked { get; set; }

        public int CheckDay { get; set; }

        /// <summary>
        /// 打卡状态描述 (Check-in/Check-out Status Description)
        /// </summary>
        public string CheckStatusDescription { get; set; }
    }
}


