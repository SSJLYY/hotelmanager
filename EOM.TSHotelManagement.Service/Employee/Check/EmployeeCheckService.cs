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
using jvncorelib.CodeLib;
using Microsoft.Extensions.Logging;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 员工打卡接口实现类
    /// </summary>
    public class EmployeeCheckService : IEmployeeCheckService
    {
        /// <summary>
        /// 员工打卡
        /// </summary>
        private readonly GenericRepository<EmployeeCheck> workerCheckRepository;

        /// <summary>
        /// 唯一编码
        /// </summary>
        private readonly UniqueCode uniqueCode;

        public EmployeeCheckService(GenericRepository<EmployeeCheck> workerCheckRepository, UniqueCode uniqueCode)
        {
            this.workerCheckRepository = workerCheckRepository;
            this.uniqueCode = uniqueCode;
        }

        /// <summary>
        /// 根据员工编号查询其所有的打卡记录
        /// </summary>
        /// <param name="wid"></param>
        /// <returns></returns>
        public ListOutputDto<ReadEmployeeCheckOutputDto> SelectCheckInfoByEmployeeId(ReadEmployeeCheckInputDto wid)
        {
            List<EmployeeCheck> workerChecks = new List<EmployeeCheck>();

            var where = SqlFilterBuilder.BuildExpression<EmployeeCheck, ReadEmployeeCheckInputDto>(wid);

            var count = 0;

            if (!wid.IgnorePaging)
            {
                workerChecks = workerCheckRepository.AsQueryable().Where(where.ToExpression())
                    .OrderByDescending(a => a.CheckTime)
                    .ToPageList(wid.Page, wid.PageSize, ref count);
            }
            else
            {
                workerChecks = workerCheckRepository.AsQueryable().Where(where.ToExpression())
                    .OrderByDescending(a => a.CheckTime)
                    .ToList();
                count = workerChecks.Count;
            }

            var source = EntityMapper.MapList<EmployeeCheck, ReadEmployeeCheckOutputDto>(workerChecks);

            source.ForEach(source =>
            {
                source.CheckStatusDescription = source.CheckStatus == 0 ? "早班" : "晚班";
                source.CheckMethodDescription = source.CheckMethod == CheckTypeConstant.Web.Code ? CheckTypeConstant.Web.Description : CheckTypeConstant.Client.Description;
            });
            return new ListOutputDto<ReadEmployeeCheckOutputDto>
            {
                Data = new PagedData<ReadEmployeeCheckOutputDto>
                {
                    Items = source,
                    TotalCount = count
                }
            };
        }


        /// <summary>
        /// 查询员工签到天数
        /// </summary>
        /// <param name="wkn"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadEmployeeCheckOutputDto> SelectWorkerCheckDaySumByEmployeeId(ReadEmployeeCheckInputDto wkn)
        {
            var checkDay = 0;
            try
            {
                checkDay = workerCheckRepository.AsQueryable().Count(a => a.EmployeeId == wkn.EmployeeId && a.IsDelete != 1);
            }
            catch (Exception)
            {
                return new SingleOutputDto<ReadEmployeeCheckOutputDto> { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Query employee sign-in days failed", "查询员工签到天数失败") };
            }

            return new SingleOutputDto<ReadEmployeeCheckOutputDto>
            {
                Data = new ReadEmployeeCheckOutputDto
                {
                    CheckDay = checkDay
                }
            };
        }


        /// <summary>
        /// 查询今天员工是否已签到
        /// </summary>
        /// <param name="wkn"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadEmployeeCheckOutputDto> SelectToDayCheckInfoByWorkerNo(ReadEmployeeCheckInputDto wkn)
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var morningChecked = workerCheckRepository.AsQueryable()
                    .Any(a => a.EmployeeId == wkn.EmployeeId
                           && a.IsDelete != 1
                           && a.CheckStatus == 0
                           && a.CheckTime >= today && a.CheckTime < tomorrow);

                var eveningChecked = workerCheckRepository.AsQueryable()
                    .Any(a => a.EmployeeId == wkn.EmployeeId
                           && a.IsDelete != 1
                           && a.CheckStatus == 1
                           && a.CheckTime >= today && a.CheckTime < tomorrow);

                var checkDay = workerCheckRepository.AsQueryable().Count(a => a.EmployeeId == wkn.EmployeeId && a.IsDelete != 1);

                return new SingleOutputDto<ReadEmployeeCheckOutputDto>
                {
                    Data = new ReadEmployeeCheckOutputDto
                    {
                        MorningChecked = morningChecked,
                        EveningChecked = eveningChecked,
                        CheckDay = checkDay
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleOutputDto<ReadEmployeeCheckOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Error:\n{ex.Message}", $"错误:\n{ex.Message}")
                };
            }
        }

        /// <summary>
        /// 添加员工打卡数据
        /// </summary>
        /// <param name="workerCheck"></param>
        /// <returns></returns>
        public BaseResponse AddCheckInfo(CreateEmployeeCheckInputDto workerCheck)
        {
            try
            {
                if (workerCheck == null || string.IsNullOrWhiteSpace(workerCheck.EmployeeId))
                {
                    return new BaseResponse
                    {
                        Message = LocalizationHelper.GetLocalizedString(
                            "Employee ID is required.",
                            "员工工号为必填字段"
                        ),
                        Code = BusinessStatusCode.InternalServerError
                    };
                }

                var employeeId = workerCheck.EmployeeId.Trim();
                var now = DateTime.Now;
                var isMorningShift = now.Hour < 12;

                var shiftStart = isMorningShift
                    ? now.Date
                    : now.Date.AddHours(12);

                var shiftEnd = isMorningShift
                    ? now.Date.AddHours(12)
                    : now.Date.AddDays(1);

                var shiftStatus = isMorningShift ? 0 : 1;
                var shiftName = isMorningShift ? "早班" : "晚班";

                var alreadyChecked = workerCheckRepository.IsAny(x =>
                    x.EmployeeId == employeeId &&
                    x.CheckStatus == shiftStatus &&
                    x.CheckTime >= shiftStart &&
                    x.CheckTime < shiftEnd
                );

                if (alreadyChecked)
                {
                    return new BaseResponse
                    {
                        Message = LocalizationHelper.GetLocalizedString(
                            $"{shiftName} already checked in.",
                            $"{shiftName}已打卡"
                        ),
                        Code = BusinessStatusCode.InternalServerError
                    };
                }

                var entity = new EmployeeCheck
                {
                    CheckNumber = uniqueCode.GetNewId("CK-"),
                    EmployeeId = employeeId,
                    CheckTime = now,
                    CheckMethod = CheckTypeConstant.Web.Code,
                    CheckStatus = shiftStatus
                };

                var result = workerCheckRepository.Insert(entity);
                if (!result)
                {
                    return new BaseResponse
                    {
                        Message = LocalizationHelper.GetLocalizedString(
                            "insert employee check failed.",
                            "员工打卡添加失败"
                        ),
                        Code = BusinessStatusCode.InternalServerError
                    };
                }

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message),
                    Code = BusinessStatusCode.InternalServerError
                };
            }
        }

    }
}
