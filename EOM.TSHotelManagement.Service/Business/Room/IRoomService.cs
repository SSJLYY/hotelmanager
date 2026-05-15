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
    /// 房间信息接口
    /// </summary>
    public interface IRoomService
    {
        #region 根据房间状态获取相应状态的房间信息
        /// <summary>
        /// 根据房间状态获取相应状态的房间信息
        /// </summary>
        /// <param name="readRoomInputDto"></param>
        /// <returns></returns>
        ListOutputDto<ReadRoomOutputDto> SelectRoomByRoomState(ReadRoomInputDto readRoomInputDto);
        #endregion

        #region 根据房间状态来查询可使用的房间
        /// <summary>
        /// 根据房间状态来查询可使用的房间
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadRoomOutputDto> SelectCanUseRoomAll();
        #endregion

        #region 获取所有房间信息
        /// <summary>
        /// 获取所有房间信息
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadRoomOutputDto> SelectRoomAll(ReadRoomInputDto readRoomInputDto);
        #endregion

        #region 获取房间分区的信息
        /// <summary>
        /// 获取房间分区的信息
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadRoomOutputDto> SelectRoomByTypeName(ReadRoomInputDto TypeName);
        #endregion

        #region 根据房间编号查询房间信息
        /// <summary>
        /// 根据房间编号查询房间信息
        /// </summary>
        /// <param name="readRoomInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadRoomOutputDto> SelectRoomByRoomNo(ReadRoomInputDto readRoomInputDto);
        #endregion

        #region 根据房间编号查询截止到今天住了多少天
        /// <summary>
        /// 根据房间编号查询截止到今天住了多少天
        /// </summary>
        /// <param name="readRoomInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadRoomOutputDto> DayByRoomNo(ReadRoomInputDto readRoomInputDto);
        #endregion

        #region 根据房间编号修改房间信息（入住）
        /// <summary>
        /// 根据房间编号修改房间信息（入住）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        BaseResponse UpdateRoomInfo(UpdateRoomInputDto r);
        #endregion

        #region 根据房间编号修改房间信息（预约）
        /// <summary>
        /// 根据房间编号修改房间信息（预约）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        BaseResponse UpdateRoomInfoWithReser(UpdateRoomInputDto r);
        #endregion

        #region 查询可入住房间数量
        /// <summary>
        /// 查询可入住房间数量
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadRoomOutputDto> SelectCanUseRoomAllByRoomState();
        #endregion

        #region 查询已入住房间数量
        /// <summary>
        /// 查询已入住房间数量
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadRoomOutputDto> SelectNotUseRoomAllByRoomState();
        #endregion

        #region 根据房间编号查询房间价格
        /// <summary>
        /// 根据房间编号查询房间价格
        /// </summary>
        /// <returns></returns>
        object SelectRoomByRoomPrice(ReadRoomInputDto readRoomInputDto);
        #endregion

        /// <summary>
        /// 查询房间可用计价项
        /// </summary>
        /// <param name="readRoomInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadRoomPricingOutputDto> SelectRoomPricingOptions(ReadRoomInputDto readRoomInputDto);

        #region 查询脏房数量
        /// <summary>
        /// 查询脏房数量
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadRoomOutputDto> SelectNotClearRoomAllByRoomState();
        #endregion

        #region 查询维修房数量
        /// <summary>
        /// 查询维修房数量
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadRoomOutputDto> SelectFixingRoomAllByRoomState();
        #endregion

        #region 查询预约房数量
        /// <summary>
        /// 查询预约房数量
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadRoomOutputDto> SelectReservedRoomAllByRoomState();
        #endregion

        #region 根据房间编号更改房间状态
        /// <summary>
        /// 根据房间编号更改房间状态
        /// </summary>
        /// <param name="readRoomInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdateRoomStateByRoomNo(UpdateRoomInputDto readRoomInputDto);
        #endregion

        /// <summary>
        /// 转房操作
        /// </summary>
        /// <param name="transferRoomDto"></param>
        /// <returns></returns>
        BaseResponse TransferRoom(TransferRoomDto transferRoomDto);

        /// <summary>
        /// 退房操作
        /// </summary>
        /// <param name="checkoutRoomDto"></param>
        /// <returns></returns>
        BaseResponse CheckoutRoom(CheckoutRoomDto checkoutRoomDto);

        /// <summary>
        /// 根据预约信息办理入住
        /// </summary>
        /// <param name="checkinRoomByReservationDto"></param>
        /// <returns></returns>
        BaseResponse CheckinRoomByReservation(CheckinRoomByReservationDto checkinRoomByReservationDto);

        #region 添加房间
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="rn"></param>
        /// <returns></returns>
        BaseResponse InsertRoom(CreateRoomInputDto rn);
        #endregion

        #region 更新房间
        /// <summary>
        /// 更新房间
        /// </summary>
        /// <param name="rn"></param>
        /// <returns></returns>
        BaseResponse UpdateRoom(UpdateRoomInputDto rn);
        #endregion

        #region 删除房间
        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="rn"></param>
        /// <returns></returns>
        BaseResponse DeleteRoom(DeleteRoomInputDto rn);
        #endregion

    }
}
