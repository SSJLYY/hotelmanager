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
    public class ImageHostingServiceCheckJob : IJob
    {
        private readonly ILogger<ImageHostingServiceCheckJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ImageHostingServiceCheckJob(
            ILogger<ImageHostingServiceCheckJob> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始检测图床服务状态...");

            using var scope = _serviceProvider.CreateScope();
            var imageHostingService = scope.ServiceProvider.GetRequiredService<LskyHelper>();

            bool isHealthy = false;
            try
            {
                isHealthy = await imageHostingService.CheckServiceStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "图床服务检测异常");
            }

            _logger.LogInformation($"图床服务检测完成，状态：{(isHealthy ? "健康" : "故障")}");
        }

    }
}
