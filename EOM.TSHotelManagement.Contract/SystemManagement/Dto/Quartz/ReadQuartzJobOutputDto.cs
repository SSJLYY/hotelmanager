namespace EOM.TSHotelManagement.Contract
{
    public class ReadQuartzJobOutputDto
    {
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public string JobDescription { get; set; }
        public bool IsDurable { get; set; }
        public bool RequestsRecovery { get; set; }

        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public string TriggerType { get; set; }
        public string TriggerState { get; set; }

        public string CronExpression { get; set; }
        public string TimeZoneId { get; set; }
        public int? RepeatCount { get; set; }
        public double? RepeatIntervalMs { get; set; }

        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public DateTime? NextFireTimeUtc { get; set; }
        public DateTime? PreviousFireTimeUtc { get; set; }
    }
}
