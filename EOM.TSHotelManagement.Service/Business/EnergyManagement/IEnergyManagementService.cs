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

/// <summary>
/// 水电信息接口
/// </summary>
public interface IEnergyManagementService
{
    /// <summary>
    /// 根据条件查询水电费信息
    /// 替换了 SelectWtiInfoByRoomNo, SelectWtiInfoByRoomNoAndTime, ListWtiInfoByRoomNo, SelectWtiInfoAll
    /// </summary>
    /// <param name="readEnergyManagementInputDto">Dto</param>
    /// <returns>符合条件的水电费信息列表</returns>
    ListOutputDto<ReadEnergyManagementOutputDto> SelectEnergyManagementInfo(ReadEnergyManagementInputDto readEnergyManagementInputDto);

    /// <summary>
    /// 添加水电费信息
    /// 替换了 InsertWtiInfo
    /// </summary>
    /// <param name="w"></param>
    /// <returns></returns>
    BaseResponse InsertEnergyManagementInfo(CreateEnergyManagementInputDto w);

    /// <summary>
    /// 修改水电费信息
    /// 替换了 UpdateWtiInfo 和 UpdateWtiInfoByRoomNoAndDateTime
    /// </summary>
    /// <param name="w">包含要修改的数据，以及WtiNo作为查询条件</param>
    /// <returns></returns>
    BaseResponse UpdateEnergyManagementInfo(UpdateEnergyManagementInputDto w);

    /// <summary>
    /// 根据房间编号、使用时间删除水电费信息
    /// 替换了 DeleteWtiInfoByRoomNoAndDateTime
    /// </summary>
    /// <param name="deleteEnergyManagementInputDto"></param>
    /// <returns></returns>
    BaseResponse DeleteEnergyManagementInfo(DeleteEnergyManagementInputDto deleteEnergyManagementInputDto);
}
