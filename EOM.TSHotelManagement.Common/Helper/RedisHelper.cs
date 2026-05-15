using EOM.TSHotelManagement.Infrastructure;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Common
{
    public class RedisHelper
    {
        private readonly object _lock = new object();
        private IConnectionMultiplexer _connection;
        private int _defaultDatabase = -1;
        private readonly ILogger<RedisHelper> logger;
        private readonly RedisConfigFactory configFactory;

        public RedisHelper(RedisConfigFactory configFactory, ILogger<RedisHelper> logger)
        {
            this.configFactory = configFactory;
            this.logger = logger;
            Initialize();
        }

        public void Initialize()
        {
            lock (_lock)
            {
                if (_connection != null)
                {
                    return;
                }

                try
                {
                    var redisConfig = configFactory.GetRedisConfig();
                    if (!redisConfig.Enable)
                    {
                        logger.LogInformation("Redis功能未启用，跳过初始化");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
                    {
                        throw new ArgumentException("Redis连接字符串不能为空");
                    }

                    _defaultDatabase = redisConfig.DefaultDatabase ?? -1;

                    var options = ConfigurationOptions.Parse(redisConfig.ConnectionString, ignoreUnknown: true);
                    options.AbortOnConnectFail = false;
                    options.ConnectTimeout = Clamp(redisConfig.ConnectTimeoutMs, 1000, 30000, 5000);
                    options.SyncTimeout = Clamp(redisConfig.SyncTimeoutMs, 500, 30000, 2000);
                    options.AsyncTimeout = Clamp(redisConfig.AsyncTimeoutMs, 500, 30000, 2000);
                    options.KeepAlive = Clamp(redisConfig.KeepAliveSeconds, 5, 300, 15);
                    options.ConnectRetry = Clamp(redisConfig.ConnectRetry, 1, 10, 3);
                    options.ReconnectRetryPolicy = new ExponentialRetry(
                        Clamp(redisConfig.ReconnectRetryBaseDelayMs, 500, 30000, 3000));

                    if (_defaultDatabase >= 0)
                    {
                        options.DefaultDatabase = _defaultDatabase;
                    }

                    _connection = ConnectionMultiplexer.Connect(options);
                    _connection.GetDatabase(_defaultDatabase).Ping();
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Redis初始化失败");
                    throw;
                }
            }
        }

        public IDatabase GetDatabase()
        {
            if (_connection == null)
            {
                throw new Exception("RedisHelper not initialized. Call Initialize first.");
            }

            return _connection.GetDatabase(_defaultDatabase);
        }

        public async Task<bool> CheckServiceStatusAsync()
        {
            var redisConfig = configFactory.GetRedisConfig();
            if (!redisConfig.Enable)
            {
                logger.LogInformation("Redis功能未启用，跳过初始化");
                return false;
            }

            if (string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
            {
                throw new ArgumentException("Redis连接字符串不能为空");
            }

            try
            {
                var db = GetDatabase();
                var ping = await db.PingAsync();
                logger.LogInformation($"Redis响应时间：{ping.TotalMilliseconds} ms");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis服务检查失败");
                return false;
            }
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                var db = GetDatabase();
                if (expiry.HasValue)
                {
                    return await db.StringSetAsync(key, value, expiry.Value);
                }

                return await db.StringSetAsync(key, value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Redis设置值失败，键：{key}！");
                return false;
            }
        }

        public async Task<string> GetAsync(string key)
        {
            try
            {
                var db = GetDatabase();
                return await db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Redis获取值失败，键：{key}！");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                var db = GetDatabase();
                return await db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Redis删除键失败，键：{key}！");
                return false;
            }
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            try
            {
                var db = GetDatabase();
                return await db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Redis键存在检查失败，键：{key}！");
                return false;
            }
        }

        private static int Clamp(int? value, int min, int max, int fallback)
        {
            return Math.Clamp(value ?? fallback, min, max);
        }
    }
}

