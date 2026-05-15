using EOM.TSHotelManagement.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Common.QuartzWorkspace.BusinessJob
{
    [DisallowConcurrentExecution]
    public class MailServiceCheckJob : IJob
    {
        private readonly ILogger<MailServiceCheckJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public MailServiceCheckJob(
            ILogger<MailServiceCheckJob> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始检测邮件服务状态...");

            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<MailHelper>();

            bool isHealthy = false;
            try
            {
                isHealthy = await emailService.CheckServiceStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邮件服务检测异常");
            }

            _logger.LogInformation($"邮件服务检测完成，状态：{(isHealthy ? "健康" : "故障")}");
        }

    }
}
