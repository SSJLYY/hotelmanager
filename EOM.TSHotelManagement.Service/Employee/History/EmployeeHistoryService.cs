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

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 员工履历接口实现类
    /// </summary>
    public class EmployeeHistoryService : IEmployeeHistoryService
    {
        /// <summary>
        /// 员工履历
        /// </summary>
        private readonly GenericRepository<EmployeeHistory> workerHistoryRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workerHistoryRepository"></param>
        public EmployeeHistoryService(GenericRepository<EmployeeHistory> workerHistoryRepository)
        {
            this.workerHistoryRepository = workerHistoryRepository;
        }

        /// <summary>
        /// 根据工号添加员工履历
        /// </summary>
        /// <param name="workerHistory"></param>
        /// <returns></returns>
        public BaseResponse AddHistoryByEmployeeId(CreateEmployeeHistoryInputDto workerHistory)
        {
            try
            {
                workerHistoryRepository.Insert(EntityMapper.Map<CreateEmployeeHistoryInputDto, EmployeeHistory>(workerHistory));
            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 根据工号更新员工履历
        /// </summary>
        /// <param name="workerHistory"></param>
        /// <returns></returns>
        public BaseResponse UpdateHistoryByEmployeeId(UpdateEmployeeHistoryInputDto workerHistory)
        {
            try
            {
                var existingHistory = workerHistoryRepository.GetSingle(a => a.Id == workerHistory.Id);
                if (existingHistory == null)
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Cannot found Employee History", "找不到员工履历"), Code = BusinessStatusCode.NotFound };
                }
                existingHistory.StartDate = workerHistory.StartDate;
                existingHistory.EndDate = workerHistory.EndDate;
                existingHistory.Company = workerHistory.Company;
                existingHistory.Position = workerHistory.Position;
                workerHistoryRepository.Update(existingHistory);
            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 根据工号查询履历信息
        /// </summary>
        /// <param name="wid"></param>
        /// <returns></returns>
        public ListOutputDto<ReadEmployeeHistoryOutputDto> SelectHistoryByEmployeeId(ReadEmployeeHistoryInputDto wid)
        {
            List<EmployeeHistory> why = workerHistoryRepository.GetList(a => a.IsDelete != 1 && a.EmployeeId == wid.EmployeeId);
            var result = EntityMapper.MapList<EmployeeHistory, ReadEmployeeHistoryOutputDto>(why);
            return new ListOutputDto<ReadEmployeeHistoryOutputDto>
            {
                Data = new PagedData<ReadEmployeeHistoryOutputDto>
                {
                    Items = result,
                    TotalCount = result.Count
                }
            };
        }
    }
}
