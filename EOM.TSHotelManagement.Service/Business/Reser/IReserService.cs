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
    /// 预约信息接口
    /// </summary>
    public interface IReserService
    {

        /// <summary>
        /// 获取所有预约信息
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadReserOutputDto> SelectReserAll(ReadReserInputDto readReserInputDto);

        /// <summary>
        /// 根据房间编号获取预约信息
        /// </summary>
        /// <param name="readReserInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadReserOutputDto> SelectReserInfoByRoomNo(ReadReserInputDto readReserInputDto);

        /// <summary>
        /// 删除预约信息
        /// </summary>
        /// <param name="reser"></param>
        /// <returns></returns>
        BaseResponse DeleteReserInfo(DeleteReserInputDto reser);

        /// <summary>
        /// 更新预约信息
        /// </summary>
        /// <param name="reser"></param>
        /// <returns></returns>
        BaseResponse UpdateReserInfo(UpdateReserInputDto reser);

        /// <summary>
        /// 添加预约信息
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        BaseResponse InserReserInfo(CreateReserInputDto r);

        /// <summary>
        /// 查询所有预约类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<EnumDto> SelectReserTypeAll();

    }
}