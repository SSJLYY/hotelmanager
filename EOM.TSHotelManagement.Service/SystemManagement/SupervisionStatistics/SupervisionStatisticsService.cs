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
    /// 监管统计接口实现类
    /// </summary>
    public class SupervisionStatisticsService : ISupervisionStatisticsService
    {
        /// <summary>
        /// 监管统计
        /// </summary>
        private readonly GenericRepository<SupervisionStatistics> checkInfoRepository;

        private readonly ILogger<SupervisionStatisticsService> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkInfoRepository"></param>
        public SupervisionStatisticsService(GenericRepository<SupervisionStatistics> checkInfoRepository, ILogger<SupervisionStatisticsService> logger)
        {
            this.checkInfoRepository = checkInfoRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 查询所有监管统计信息
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadSupervisionStatisticsOutputDto> SelectSupervisionStatisticsAll(ReadSupervisionStatisticsInputDto readSupervisionStatisticsInputDto)
        {
            List<SupervisionStatistics> cif = new List<SupervisionStatistics>();

            var where = SqlFilterBuilder.BuildExpression<SupervisionStatistics, ReadSupervisionStatisticsInputDto>(readSupervisionStatisticsInputDto, nameof(SupervisionStatistics.SupervisionTime));

            var count = 0;

            if (!readSupervisionStatisticsInputDto.IgnorePaging)
            {
                cif = checkInfoRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readSupervisionStatisticsInputDto.Page, readSupervisionStatisticsInputDto.PageSize, ref count);
            }
            else
            {
                cif = checkInfoRepository.AsQueryable().Where(where.ToExpression()).ToList();
            }

            var deptId = cif.Select(a => a.SupervisingDepartment).ToList();
            var depts = checkInfoRepository.Change<Department>().GetList(a => deptId.Contains(a.DepartmentNumber));
            cif.ForEach(c =>
            {
                var dept = depts.SingleOrDefault(a => a.DepartmentNumber == c.SupervisingDepartment);
                c.SupervisingDepartmentName = !dept.IsNullOrEmpty() ? dept.DepartmentName : string.Empty;
            });

            var Data = EntityMapper.MapList<SupervisionStatistics, ReadSupervisionStatisticsOutputDto>(cif);

            return new ListOutputDto<ReadSupervisionStatisticsOutputDto>
            {
                Data = new PagedData<ReadSupervisionStatisticsOutputDto>
                {
                    Items = Data,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 插入监管统计信息
        /// </summary>
        /// <param name="checkInfo"></param>
        /// <returns></returns>
        public BaseResponse InsertSupervisionStatistics(CreateSupervisionStatisticsInputDto checkInfo)
        {
            try
            {
                checkInfoRepository.Insert(EntityMapper.Map<CreateSupervisionStatisticsInputDto, SupervisionStatistics>(checkInfo));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while inserting Supervision Statistics.");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 更新监管统计信息
        /// </summary>
        /// <param name="checkInfo"></param>
        /// <returns></returns>
        public BaseResponse UpdateSupervisionStatistics(UpdateSupervisionStatisticsInputDto checkInfo)
        {
            try
            {
                var supervisionStatistics = checkInfoRepository.GetFirst(a => a.StatisticsNumber == checkInfo.StatisticsNumber);
                supervisionStatistics.SupervisingDepartment = checkInfo.SupervisingDepartment;
                supervisionStatistics.SupervisionLoss = checkInfo.SupervisionLoss;
                supervisionStatistics.SupervisionScore = checkInfo.SupervisionScore;
                supervisionStatistics.SupervisionAdvice = checkInfo.SupervisionAdvice;
                supervisionStatistics.SupervisionStatistician = checkInfo.SupervisionStatistician;
                supervisionStatistics.SupervisionProgress = checkInfo.SupervisionProgress;
                supervisionStatistics.SupervisionTime = checkInfo.SupervisionTime ?? DateTime.MinValue;
                supervisionStatistics.IsDelete = checkInfo.IsDelete;
                supervisionStatistics.RowVersion = checkInfo.RowVersion ?? 0;

                var updateResult = checkInfoRepository.Update(supervisionStatistics);
                if (!updateResult)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating Supervision Statistics.");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除监管统计信息
        /// </summary>
        /// <param name="checkInfo"></param>
        /// <returns></returns>
        public BaseResponse DeleteSupervisionStatistics(DeleteSupervisionStatisticsInputDto input)
        {
            try
            {
                if (input?.DelIds == null || !input.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(input);
                var supervisionStatistics = checkInfoRepository.GetList(a => delIds.Contains(a.Id));

                if (!supervisionStatistics.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Supervision Statistics Information Not Found", "监管统计信息未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(input, supervisionStatistics, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 批量软删除
                var result = checkInfoRepository.SoftDeleteRange(supervisionStatistics);

                if (result)
                {
                    return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Delete Supervision Statistics Success", "监管统计信息删除成功"));
                }
                else
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Delete Supervision Statistics Failed", "监管统计信息删除失败"));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting Supervision Statistics.");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }
    }
}
