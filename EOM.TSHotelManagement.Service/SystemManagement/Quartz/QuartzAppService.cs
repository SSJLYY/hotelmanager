using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using Quartz;
using Quartz.Impl.Matchers;

namespace EOM.TSHotelManagement.Service
{
    public class QuartzAppService : IQuartzAppService
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzAppService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task<ListOutputDto<ReadQuartzJobOutputDto>> SelectQuartzJobList()
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
                var rows = new List<ReadQuartzJobOutputDto>();

                foreach (var jobKey in jobKeys.OrderBy(a => a.Group).ThenBy(a => a.Name))
                {
                    var jobDetail = await scheduler.GetJobDetail(jobKey);
                    if (jobDetail == null)
                    {
                        continue;
                    }

                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    if (triggers == null || triggers.Count == 0)
                    {
                        rows.Add(new ReadQuartzJobOutputDto
                        {
                            JobName = jobKey.Name,
                            JobGroup = jobKey.Group,
                            JobDescription = jobDetail.Description,
                            IsDurable = jobDetail.Durable,
                            RequestsRecovery = jobDetail.RequestsRecovery
                        });
                        continue;
                    }

                    foreach (var trigger in triggers.OrderBy(a => a.Key.Group).ThenBy(a => a.Key.Name))
                    {
                        var state = await scheduler.GetTriggerState(trigger.Key);
                        var cronTrigger = trigger as ICronTrigger;
                        var simpleTrigger = trigger as ISimpleTrigger;

                        rows.Add(new ReadQuartzJobOutputDto
                        {
                            JobName = jobKey.Name,
                            JobGroup = jobKey.Group,
                            JobDescription = jobDetail.Description,
                            IsDurable = jobDetail.Durable,
                            RequestsRecovery = jobDetail.RequestsRecovery,
                            TriggerName = trigger.Key.Name,
                            TriggerGroup = trigger.Key.Group,
                            TriggerType = trigger.GetType().Name,
                            TriggerState = state.ToString(),
                            CronExpression = cronTrigger?.CronExpressionString,
                            TimeZoneId = cronTrigger?.TimeZone?.Id,
                            RepeatCount = simpleTrigger?.RepeatCount,
                            RepeatIntervalMs = simpleTrigger?.RepeatInterval.TotalMilliseconds,
                            StartTimeUtc = trigger.StartTimeUtc.UtcDateTime,
                            EndTimeUtc = trigger.EndTimeUtc?.UtcDateTime,
                            NextFireTimeUtc = trigger.GetNextFireTimeUtc()?.UtcDateTime,
                            PreviousFireTimeUtc = trigger.GetPreviousFireTimeUtc()?.UtcDateTime
                        });
                    }
                }

                return new ListOutputDto<ReadQuartzJobOutputDto>
                {
                    Data = new PagedData<ReadQuartzJobOutputDto>
                    {
                        Items = rows,
                        TotalCount = rows.Count
                    }
                };
            }
            catch (Exception ex)
            {
                return new ListOutputDto<ReadQuartzJobOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message),
                    Data = new PagedData<ReadQuartzJobOutputDto>
                    {
                        Items = new List<ReadQuartzJobOutputDto>(),
                        TotalCount = 0
                    }
                };
            }
        }
    }
}
