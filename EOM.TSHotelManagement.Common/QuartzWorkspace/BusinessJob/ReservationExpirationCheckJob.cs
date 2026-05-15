using EOM.TSHotelManagement.Common.Helper;
using EOM.TSHotelManagement.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Common.QuartzWorkspace.BusinessJob
{
    [DisallowConcurrentExecution]
    public class ReservationExpirationCheckJob : IJob
    {
        private readonly ILogger<ReservationExpirationCheckJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ReservationExpirationCheckJob(
            ILogger<ReservationExpirationCheckJob> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始检查数据过期时间...");

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            var emailService = scope.ServiceProvider.GetRequiredService<MailHelper>();

            var isHealthForMailService = await emailService.CheckServiceStatusAsync();

            if (!isHealthForMailService)
            {
                _logger.LogWarning("邮件服务不可用，跳过发送过期提醒邮件的步骤。");
            }

            // 获取配置的提前提醒天数
            var notifyDays = _configuration.GetValue<int>("ExpirationSettings:NotifyDaysBefore", 3);
            var currentDate = DateOnly.FromDateTime(DateTime.Now.Date);
            var expirationThreshold = currentDate.AddDays(notifyDays);

            // 查询即将过期的数据
            var expiringItems = db.Queryable<Reser>()
                .Where(r => r.ReservationEndDate <= expirationThreshold && r.ReservationEndDate > currentDate && r.IsDelete != 1)
                .ToList();

            var roomNumbers = expiringItems.Select(a => a.ReservationRoomNumber).ToList();
            var rooms = db.Queryable<Room>().Where(a => roomNumbers.Contains(a.RoomNumber) && a.IsDelete != 1).ToList();
            var types = rooms.Select(a => a.RoomTypeId).ToList();
            var roomTypes = db.Queryable<RoomType>().Where(a => types.Contains(a.RoomTypeId) && a.IsDelete != 1).ToList();

            var shouldReleaseRooms = new List<string>();
            var shouldDeleteReservations = new List<int>();

            foreach (var item in expiringItems)
            {
                var room = rooms.FirstOrDefault(a => a.RoomNumber == item.ReservationRoomNumber);
                var roomType = roomTypes.FirstOrDefault(a => a.RoomTypeId == room?.RoomTypeId);
                try
                {
                    var endDate = item.ReservationEndDate.ToDateTime(new TimeOnly(0, 0));
                    var daysLeft = (endDate - DateTime.Now.Date).Days;

                    if (daysLeft <= 0)
                    {
                        shouldReleaseRooms.Add(item.ReservationRoomNumber);
                        shouldDeleteReservations.Add(item.Id);
                    }

                    var channelDescription = EnumHelper.GetDescriptionByName<ReserType>(item.ReservationChannel);

                    var mailTemplate = EmailTemplate.SendReservationExpirationNotificationTemplate(
                        item.ReservationRoomNumber,
                        channelDescription ?? "我店",
                        item.CustomerName ?? "先生/女士",
                        roomType.RoomTypeName ?? "房间",
                        endDate,
                        daysLeft
                    );

                    if (isHealthForMailService)
                    {
                        emailService.SendMail(
                            null, // To-do: Add recipient email address
                            mailTemplate.Subject,
                            mailTemplate.Body
                        );
                    }
                    _logger.LogInformation("已为 {ItemName} 发送过期提醒", item.ReservationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "处理项目 {ItemId} 时出错", item.Id);
                }
            }

            if (shouldDeleteReservations.Any() && shouldReleaseRooms.Any())
            {
                db.Updateable<Room>()
                    .SetColumns(r => new Room { RoomStateId = (int)RoomState.Vacant })
                    .Where(r => shouldReleaseRooms.Contains(r.RoomNumber))
                    .ExecuteCommand();
                db.Updateable<Reser>()
                    .SetColumns(r => new Reser { ReservationStatus = 2 })
                    .Where(r => shouldDeleteReservations.Contains(r.Id))
                    .ExecuteCommand();
            }

            _logger.LogInformation("数据过期检查完成，共处理 {Count} 个项目", expiringItems.Count);
        }
    }
}
