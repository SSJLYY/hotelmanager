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
using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 消费信息接口
    /// </summary>
    public interface ISpendService
    {
        #region 根据房间编号查询消费信息
        /// <summary>
        /// 根据房间编号查询消费信息
        /// </summary>
        /// <param name="readSpendInputDto"></param>
        /// <returns></returns>
        ListOutputDto<ReadSpendOutputDto> SelectSpendByRoomNo(ReadSpendInputDto readSpendInputDto);
        #endregion

        #region 根据客户编号查询历史消费信息
        /// <summary>
        /// 根据客户编号查询历史消费信息
        /// </summary>
        /// <param name="readSpendInputDto"></param>
        /// <returns></returns>
        ListOutputDto<ReadSpendOutputDto> SeletHistorySpendInfoAll(ReadSpendInputDto readSpendInputDto);
        #endregion

        #region 查询消费的所有信息
        /// <summary>
        /// 查询消费的所有信息
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadSpendOutputDto> SelectSpendInfoAll(ReadSpendInputDto readSpendInputDto);
        #endregion

        #region 根据房间编号、入住时间到当前时间查询消费总金额
        /// <summary>
        /// 根据房间编号、入住时间到当前时间查询消费总金额
        /// </summary>
        /// <param name="readSpendInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadSpendInputDto> SumConsumptionAmount(ReadSpendInputDto readSpendInputDto);
        #endregion

        /// <summary>
        /// 撤回客户消费信息
        /// </summary>
        /// <param name="undoCustomerSpendInputDto"></param>
        /// <returns></returns>
        BaseResponse UndoCustomerSpend(UndoCustomerSpendInputDto undoCustomerSpendInputDto);

        /// <summary>
        /// 添加客户消费信息
        /// </summary>
        /// <param name="addCustomerSpendInputDto"></param>
        /// <returns></returns>
        BaseResponse AddCustomerSpend(AddCustomerSpendInputDto addCustomerSpendInputDto);

        /// <summary>
        /// 更新消费信息
        /// </summary>
        /// <param name="updateSpendInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdSpendInfo(UpdateSpendInputDto updateSpendInputDto);
    }
}
