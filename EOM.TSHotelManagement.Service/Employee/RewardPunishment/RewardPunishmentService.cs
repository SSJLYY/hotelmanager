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
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 员工奖惩记录接口实现类
    /// </summary>
    public class RewardPunishmentService : IRewardPunishmentService
    {
        /// <summary>
        /// 员工奖惩记录
        /// </summary>
        private readonly GenericRepository<EmployeeRewardPunishment> rewardPunishmentRepository;

        /// <summary>
        /// 管理员记录
        /// </summary>
        private readonly GenericRepository<Administrator> adminRepository;

        /// <summary>
        /// 奖惩类型
        /// </summary>
        private readonly GenericRepository<RewardPunishmentType> goodbadTypeRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rewardPunishmentRepository"></param>
        /// <param name="adminRepository"></param>
        /// <param name="goodbadTypeRepository"></param>
        public RewardPunishmentService(GenericRepository<EmployeeRewardPunishment> rewardPunishmentRepository, GenericRepository<Administrator> adminRepository, GenericRepository<RewardPunishmentType> goodbadTypeRepository)
        {
            this.rewardPunishmentRepository = rewardPunishmentRepository;
            this.adminRepository = adminRepository;
            this.goodbadTypeRepository = goodbadTypeRepository;
        }

        /// <summary>
        /// 添加员工奖惩记录
        /// </summary>
        /// <param name="goodBad"></param>
        /// <returns></returns>
        public BaseResponse AddRewardPunishment(CreateEmployeeRewardPunishmentInputDto goodBad)
        {
            try
            {
                var entity = EntityMapper.Map<CreateEmployeeRewardPunishmentInputDto, EmployeeRewardPunishment>(goodBad);
                var result = rewardPunishmentRepository.Insert(entity);
                if (!result)
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Add reward punishment failed", "员工奖惩记录添加失败"), Code = BusinessStatusCode.InternalServerError };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 根据工号查找所有的奖惩记录信息
        /// </summary>
        /// <param name="wn"></param>
        /// <returns></returns>
        public ListOutputDto<ReadEmployeeRewardPunishmentOutputDto> SelectAllRewardPunishmentByEmployeeId(ReadEmployeeRewardPunishmentInputDto wn)
        {
            var where = SqlFilterBuilder.BuildExpression<EmployeeRewardPunishment, ReadEmployeeRewardPunishmentInputDto>(wn);

            //查询所有超级管理员
            List<Administrator> admins = adminRepository.GetList(a => a.IsDelete != 1);
            List<RewardPunishmentType> gBTypes = goodbadTypeRepository.GetList(a => a.IsDelete != 1);
            List<EmployeeRewardPunishment> gb = new List<EmployeeRewardPunishment>();

            var count = 0;

            if (!wn.IgnorePaging)
            {
                gb = rewardPunishmentRepository.AsQueryable().Where(where.ToExpression())
                    .OrderByDescending(a => a.RewardPunishmentTime)
                    .ToPageList(wn.Page, wn.PageSize, ref count);
            }
            else
            {
                gb = rewardPunishmentRepository.AsQueryable().Where(where.ToExpression())
                    .OrderByDescending(a => a.RewardPunishmentTime)
                    .ToList();
                count = gb.Count;
            }

            gb.ForEach(source =>
            {
                //奖惩类型
                var gbType = gBTypes.SingleOrDefault(a => a.RewardPunishmentTypeId == source.RewardPunishmentType);
                source.RewardPunishmentTypeName = gbType.RewardPunishmentTypeName.IsNullOrEmpty() ? "" : gbType.RewardPunishmentTypeName;

                //操作人
                var admin = admins.SingleOrDefault(a => a.Account == source.RewardPunishmentOperator);
                source.OperatorName = admin.Name.IsNullOrEmpty() ? "" : admin.Name;
            });

            var mapped = EntityMapper.MapList<EmployeeRewardPunishment, ReadEmployeeRewardPunishmentOutputDto>(gb);

            return new ListOutputDto<ReadEmployeeRewardPunishmentOutputDto>
            {
                Data = new PagedData<ReadEmployeeRewardPunishmentOutputDto>
                {
                    Items = mapped,
                    TotalCount = count
                }
            };
        }
    }
}
