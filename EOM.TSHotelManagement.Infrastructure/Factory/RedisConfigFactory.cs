using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Infrastructure
{
    public class RedisConfigFactory
    {
        private readonly IConfiguration _configuration;

        public RedisConfigFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public RedisConfig GetRedisConfig()
        {
            var redisSection = _configuration.GetSection("Redis");
            var enable = redisSection.GetValue<bool?>("Enable")
                ?? redisSection.GetValue<bool?>("Enabled")
                ?? false;

            var redisConfig = new RedisConfig
            {
                ConnectionString = redisSection.GetValue<string>("ConnectionString"),
                Enable = enable,
                DefaultDatabase = redisSection.GetValue<int?>("DefaultDatabase"),
                ConnectTimeoutMs = redisSection.GetValue<int?>("ConnectTimeoutMs"),
                AsyncTimeoutMs = redisSection.GetValue<int?>("AsyncTimeoutMs"),
                SyncTimeoutMs = redisSection.GetValue<int?>("SyncTimeoutMs"),
                KeepAliveSeconds = redisSection.GetValue<int?>("KeepAliveSeconds"),
                ConnectRetry = redisSection.GetValue<int?>("ConnectRetry"),
                ReconnectRetryBaseDelayMs = redisSection.GetValue<int?>("ReconnectRetryBaseDelayMs"),
                OperationTimeoutMs = redisSection.GetValue<int?>("OperationTimeoutMs"),
                FailureCooldownSeconds = redisSection.GetValue<int?>("FailureCooldownSeconds")
            };
            return redisConfig;
        }
    }
}
