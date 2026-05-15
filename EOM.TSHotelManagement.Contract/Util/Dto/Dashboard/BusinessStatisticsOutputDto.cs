using System.Text.Json.Serialization;

namespace EOM.TSHotelManagement.Contract
{
    public class BusinessStatisticsOutputDto
    {
        [JsonPropertyName("genderRatio")]
        public TempGenderRatio GenderRatio { get; set; }

        [JsonPropertyName("memberTypes")]
        public List<TempMemberType> MemberTypes { get; set; }

        [JsonPropertyName("dailyConsumption")]
        public TempDailyConsumption DailyConsumption { get; set; }

        [JsonPropertyName("weeklyConsumption")]
        public TempDailyConsumption WeeklyConsumption { get; set; }

        [JsonPropertyName("yearConsumption")]
        public TempDailyConsumption YearConsumption { get; set; }

        [JsonPropertyName("totalConsumption")]
        public TempDailyConsumption TotalConsumption { get; set; }
    }

    public class TempGenderRatio
    {
        [JsonPropertyName("male")]
        public int Male { get; set; }

        [JsonPropertyName("female")]
        public int Female { get; set; }
    }

    public class TempMemberType
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class TempDailyConsumption
    {
        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("settled")]
        public decimal Settled { get; set; }
    }
}
