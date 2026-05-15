/*
 * MIT License
 *Copyright (c) 2021 易开元(Easy-Open-Meta)

 *Permission is hereby granted, free of charge, to any person obtaining a copy
 *of this software and associated documentation files (the "Software"), to deal
 *in the Software without restriction, including without limitation the rights
 *to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *copies of the Software, and to permit persons to whom the Software is
 *furnished to do so, subject to the following conditions:

 *The above copyright notice and this permission notice shall be included in all
 *copies or substantial portions of the Software.

 *THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *SOFTWARE.
 *
 */
using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 会员等级规则功能模块接口实现类
    /// </summary>
    public class VipRuleAppService : IVipRuleAppService
    {
        /// <summary>
        /// 会员等级规则
        /// </summary>
        private readonly GenericRepository<VipLevelRule> vipRuleRepository;

        /// <summary>
        /// 客户类型
        /// </summary>
        private readonly GenericRepository<CustoType> custoTypeRepository;

        private readonly ILogger<VipRuleAppService> logger;

        public VipRuleAppService(GenericRepository<VipLevelRule> vipRuleRepository, GenericRepository<CustoType> custoTypeRepository, ILogger<VipRuleAppService> logger)
        {
            this.vipRuleRepository = vipRuleRepository;
            this.custoTypeRepository = custoTypeRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 查询会员等级规则列表
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadVipLevelRuleOutputDto> SelectVipRuleList(ReadVipLevelRuleInputDto readVipLevelRuleInputDto)
        {
            List<VipLevelRule> vipRules = new List<VipLevelRule>();
            var where = SqlFilterBuilder.BuildExpression<VipLevelRule, ReadVipLevelRuleInputDto>(readVipLevelRuleInputDto ?? new ReadVipLevelRuleInputDto());

            var count = 0;
            var data = new List<VipLevelRule>();
            if (readVipLevelRuleInputDto.IgnorePaging)
            {
                data = vipRuleRepository.AsQueryable().Where(where.ToExpression()).ToList();
            }
            else
            {
                data = vipRuleRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readVipLevelRuleInputDto.Page, readVipLevelRuleInputDto.PageSize, ref count);
            }

            var listUserType = custoTypeRepository.GetList(a => a.IsDelete != 1);

            data.ForEach(source =>
            {
                var userType = listUserType.SingleOrDefault(a => a.CustomerType == source.VipLevelId);
                source.VipLevelName = userType == null ? "" : userType.CustomerTypeName;
            });

            var viprules = EntityMapper.MapList<VipLevelRule, ReadVipLevelRuleOutputDto>(data);

            return new ListOutputDto<ReadVipLevelRuleOutputDto>
            {
                Data = new PagedData<ReadVipLevelRuleOutputDto>
                {
                    Items = viprules,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询会员等级规则
        /// </summary>
        /// <param name="vipRule"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadVipLevelRuleOutputDto> SelectVipRule(ReadVipLevelRuleInputDto vipRule)
        {
            VipLevelRule vipRule1 = new VipLevelRule();

            var source = vipRuleRepository.GetFirst(a => a.RuleSerialNumber.Equals(vipRule.RuleSerialNumber));

            var userType = custoTypeRepository.GetFirst(a => a.CustomerType == source.VipLevelId);
            source.VipLevelName = userType == null ? "" : userType.CustomerTypeName;

            var vipLevel = EntityMapper.Map<VipLevelRule, ReadVipLevelRuleOutputDto>(source);

            return new SingleOutputDto<ReadVipLevelRuleOutputDto>
            {
                Data = vipLevel
            };
        }

        /// <summary>
        /// 添加会员等级规则
        /// </summary>
        /// <param name="vipRule"></param>
        /// <returns></returns>
        public BaseResponse AddVipRule(CreateVipLevelRuleInputDto vipRule)
        {
            try
            {
                vipRuleRepository.Insert(new VipLevelRule()
                {
                    RuleSerialNumber = vipRule.RuleSerialNumber,
                    RuleName = vipRule.RuleName,
                    RuleValue = vipRule.RuleValue,
                    VipLevelId = vipRule.VipLevelId,
                    IsDelete = 0,
                    DataInsUsr = vipRule.DataInsUsr,
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "添加会员等级规则失败");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除会员等级规则
        /// </summary>
        /// <param name="vipRule"></param>
        /// <returns></returns>
        public BaseResponse DelVipRule(DeleteVipLevelRuleInputDto vipRule)
        {
            try
            {
                if (vipRule?.DelIds == null || !vipRule.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(vipRule);
                var vipLevelRules = vipRuleRepository.GetList(a => delIds.Contains(a.Id));

                if (!vipLevelRules.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Vip Rule Not Found", "会员规则未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(vipRule, vipLevelRules, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 批量软删除
                vipRuleRepository.SoftDeleteRange(vipLevelRules);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除会员等级规则失败");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新会员等级规则
        /// </summary>
        /// <param name="vipRule"></param>
        /// <returns></returns>
        public BaseResponse UpdVipRule(UpdateVipLevelRuleInputDto vipRule)
        {
            try
            {
                var dbVipRule = vipRuleRepository.GetFirst(a => a.Id == vipRule.Id);
                if (dbVipRule == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("Vip Rule Not Found", "会员规则未找到"));
                }
                dbVipRule.RuleName = vipRule.RuleName;
                dbVipRule.RuleValue = vipRule.RuleValue;
                dbVipRule.VipLevelId = vipRule.VipLevelId;
                dbVipRule.IsDelete = vipRule.IsDelete;
                dbVipRule.RowVersion = vipRule.RowVersion ?? 0;
                var updateResult = vipRuleRepository.Update(dbVipRule);
                if (!updateResult)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "更新会员等级规则失败");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }
    }
}
