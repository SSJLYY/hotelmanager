using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EOM.TSHotelManagement.Contract
{
    public class HumanResourcesOutputDto
    {
        [JsonPropertyName("totalEmployees")]
        public int TotalEmployees { get; set; }

        [JsonPropertyName("totalDepartments")]
        public int TotalDepartments { get; set; }

        [JsonPropertyName("attendance")]
        [Required(ErrorMessage = "考勤记录为必填事项")]
        public TempAttendanceRecord Attendance { get; set; }
    }

    public class TempAttendanceRecord
    {
        [JsonPropertyName("morningPresent")]
        public int MorningPresent { get; set; }

        [JsonPropertyName("eveningPresent")]
        public int EveningPresent { get; set; }

        [JsonPropertyName("late")]
        public int Late { get; set; }

        [JsonPropertyName("absent")]
        public int Absent { get; set; }
    }
}
