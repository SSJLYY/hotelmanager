using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Common.QuartzWorkspace.BusinessJob
{
    [DisallowConcurrentExecution]
    public class RedisServiceCheckJob : IJob
    {
        private readonly ILogger<RedisServiceCheckJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public RedisServiceCheckJob(
            ILogger<RedisServiceCheckJob> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始检测Redis服务状态...");

            using var scope = _serviceProvider.CreateScope();
            var redisHelper = scope.ServiceProvider.GetRequiredService<RedisHelper>();

            bool isHealthy = false;
            try
            {
                isHealthy = await redisHelper.CheckServiceStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis服务检测异常");
            }

            _logger.LogInformation($"Redis服务检测完成，状态：{(isHealthy ? "健康" : "故障")}");
        }

    }
}
