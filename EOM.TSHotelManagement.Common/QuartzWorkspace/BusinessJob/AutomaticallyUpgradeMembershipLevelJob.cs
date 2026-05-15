using EOM.TSHotelManagement.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Common.QuartzWorkspace.BusinessJob
{
    [DisallowConcurrentExecution]
    public class AutomaticallyUpgradeMembershipLevelJob : IJob
    {
        private readonly ILogger<AutomaticallyUpgradeMembershipLevelJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AutomaticallyUpgradeMembershipLevelJob(
            ILogger<AutomaticallyUpgradeMembershipLevelJob> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始批量处理会员等级升级。");

            try
            {
                var result = ValidateAndUpdateCustomerInfo();
                _logger.LogInformation(
                    "会员等级升级完成，扫描客户 {ScannedCount} 个，需升级 {NeedUpgradeCount} 个，实际更新 {UpdatedCount} 个。",
                    result.ScannedCount,
                    result.NeedUpgradeCount,
                    result.UpdatedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理会员等级升级时发生异常");
            }

            return Task.CompletedTask;
        }

        private MembershipUpgradeResult ValidateAndUpdateCustomerInfo()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

            var listVipRules = db.Queryable<VipLevelRule>()
                .Where(v => v.IsDelete != 1)
                .OrderByDescending(a => a.RuleValue)
                .ToList();

            if (listVipRules.Count == 0)
            {
                _logger.LogWarning("会员等级升级已跳过：未找到有效的会员规则。");
                return MembershipUpgradeResult.Empty;
            }

            var enabledCustomerTypes = db.Queryable<CustoType>()
                .Where(x => x.IsDelete != 1)
                .Select(x => x.CustomerType)
                .ToList()
                .ToHashSet();

            listVipRules = listVipRules
                .Where(x => enabledCustomerTypes.Contains(x.VipLevelId))
                .ToList();

            if (listVipRules.Count == 0)
            {
                _logger.LogWarning("会员等级升级已跳过：规则指向的会员等级不存在或已被删除。");
                return MembershipUpgradeResult.Empty;
            }

            var customerSpends = db.Queryable<Spend>()
                .Where(s => s.IsDelete != 1 && s.CustomerNumber != null && s.CustomerNumber != string.Empty)
                .GroupBy(s => s.CustomerNumber)
                .Select((s) => new CustomerSpendAggregate
                {
                    CustomerNumber = s.CustomerNumber,
                    TotalConsumptionAmount = SqlFunc.AggregateSum(s.ConsumptionAmount)
                })
                .ToList();

            if (customerSpends.Count == 0)
            {
                _logger.LogInformation("会员等级升级已跳过：未找到有效消费记录。");
                return MembershipUpgradeResult.Empty;
            }

            var customerNumbers = customerSpends
                .Select(x => x.CustomerNumber)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var customers = db.Queryable<Customer>()
                .Where(c => c.IsDelete != 1 && customerNumbers.Contains(c.CustomerNumber))
                .ToList();

            var customerLookup = customers
                .Where(c => !string.IsNullOrWhiteSpace(c.CustomerNumber))
                .GroupBy(c => c.CustomerNumber)
                .ToDictionary(g => g.Key, g => g.First());

            var updateTime = DateTime.Now;
            var customerToUpdate = new List<Customer>();

            foreach (var customerSpend in customerSpends)
            {
                if (string.IsNullOrWhiteSpace(customerSpend.CustomerNumber))
                {
                    continue;
                }

                if (!customerLookup.TryGetValue(customerSpend.CustomerNumber, out var customer))
                {
                    continue;
                }

                var targetVipLevel = listVipRules
                    .FirstOrDefault(vipRule => customerSpend.TotalConsumptionAmount >= vipRule.RuleValue)?
                    .VipLevelId ?? 0;

                if (targetVipLevel <= 0 || targetVipLevel == customer.CustomerType)
                {
                    continue;
                }

                customer.CustomerType = targetVipLevel;
                customer.DataChgDate = updateTime;
                customer.DataChgUsr = "BatchJobSystem";
                customerToUpdate.Add(customer);
            }

            if (customerToUpdate.Count == 0)
            {
                return new MembershipUpgradeResult
                {
                    ScannedCount = customerSpends.Count,
                    NeedUpgradeCount = 0,
                    UpdatedCount = 0
                };
            }

            var updatedCount = 0;

            db.Ado.BeginTran();
            try
            {
                foreach (var batch in customerToUpdate.Chunk(200))
                {
                    var currentBatch = batch.ToList();
                    updatedCount += db.Updateable(currentBatch)
                        .UpdateColumns(
                            nameof(Customer.CustomerType),
                            nameof(Customer.DataChgDate),
                            nameof(Customer.DataChgUsr))
                        .ExecuteCommand();
                }

                db.Ado.CommitTran();
            }
            catch
            {
                db.Ado.RollbackTran();
                throw;
            }

            return new MembershipUpgradeResult
            {
                ScannedCount = customerSpends.Count,
                NeedUpgradeCount = customerToUpdate.Count,
                UpdatedCount = updatedCount
            };
        }

        private sealed class CustomerSpendAggregate
        {
            public string CustomerNumber { get; set; } = string.Empty;
            public decimal TotalConsumptionAmount { get; set; }
        }

        private sealed class MembershipUpgradeResult
        {
            public static MembershipUpgradeResult Empty { get; } = new MembershipUpgradeResult();

            public int ScannedCount { get; set; }
            public int NeedUpgradeCount { get; set; }
            public int UpdatedCount { get; set; }
        }
    }
}
