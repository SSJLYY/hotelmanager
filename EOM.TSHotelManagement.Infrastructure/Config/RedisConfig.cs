namespace EOM.TSHotelManagement.Infrastructure
{
    public class RedisConfig
    {
        public string ConnectionString { get; set; }
        public bool Enable { get; set; }
        public int? DefaultDatabase { get; set; }
        public int? ConnectTimeoutMs { get; set; }
        public int? AsyncTimeoutMs { get; set; }
        public int? SyncTimeoutMs { get; set; }
        public int? KeepAliveSeconds { get; set; }
        public int? ConnectRetry { get; set; }
        public int? ReconnectRetryBaseDelayMs { get; set; }
        public int? OperationTimeoutMs { get; set; }
        public int? FailureCooldownSeconds { get; set; }
    }
}
