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
    /// 基础信息接口
    /// </summary>
    public interface IBaseService
    {
        #region 预约类型模块

        /// <summary>
        /// 查询所有预约类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<EnumDto> SelectReserTypeAll();

        #endregion

        #region 性别模块

        /// <summary>
        /// 查询所有性别类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<EnumDto> SelectGenderTypeAll();

        #endregion

        #region 面貌模块

        /// <summary>
        /// 查询所有面貌类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<EnumDto> SelectWorkerFeatureAll();
        #endregion

        #region 房间状态模块
        /// <summary>
        /// 获取所有房间状态
        /// </summary>
        /// <returns></returns>
        ListOutputDto<EnumDto> SelectRoomStateAll();
        #endregion

        #region 职位模块

        /// <summary>
        /// 查询所有职位类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadPositionOutputDto> SelectPositionAll(ReadPositionInputDto position = null);

        /// <summary>
        /// 查询职位类型
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadPositionOutputDto> SelectPosition(ReadPositionInputDto position);

        /// <summary>
        /// 添加职位类型
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BaseResponse AddPosition(CreatePositionInputDto position);

        /// <summary>
        /// 删除职位类型
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BaseResponse DelPosition(DeletePositionInputDto position);

        /// <summary>
        /// 更新职位类型
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BaseResponse UpdPosition(UpdatePositionInputDto position);

        #endregion

        #region 民族模块

        /// <summary>
        /// 查询所有民族类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadNationOutputDto> SelectNationAll(ReadNationInputDto nation = null);

        /// <summary>
        /// 查询民族类型
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadNationOutputDto> SelectNation(ReadNationInputDto nation);

        /// <summary>
        /// 添加民族类型
        /// </summary>
        /// <param name="nation"></param>
        /// <returns></returns>
        BaseResponse AddNation(CreateNationInputDto nation);

        /// <summary>
        /// 删除民族类型
        /// </summary>
        /// <param name="nation"></param>
        /// <returns></returns>
        BaseResponse DelNation(DeleteNationInputDto nation);

        /// <summary>
        /// 更新民族类型
        /// </summary>
        /// <param name="nation"></param>
        /// <returns></returns>
        BaseResponse UpdNation(UpdateNationInputDto nation);

        #endregion

        #region 学历模块

        /// <summary>
        /// 查询所有学历类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadEducationOutputDto> SelectEducationAll(ReadEducationInputDto education = null);

        /// <summary>
        /// 查询学历类型
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadEducationOutputDto> SelectEducation(ReadEducationInputDto education);

        /// <summary>
        /// 添加学历类型
        /// </summary>
        /// <param name="education"></param>
        /// <returns></returns>
        BaseResponse AddEducation(CreateEducationInputDto education);

        /// <summary>
        /// 删除学历类型
        /// </summary>
        /// <param name="education"></param>
        /// <returns></returns>
        BaseResponse DelEducation(DeleteEducationInputDto education);

        /// <summary>
        /// 更新学历类型
        /// </summary>
        /// <param name="education"></param>
        /// <returns></returns>
        BaseResponse UpdEducation(UpdateEducationInputDto education);

        #endregion

        #region 部门模块

        /// <summary>
        /// 查询所有部门类型(可用)
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadDepartmentOutputDto> SelectDeptAllCanUse();

        /// <summary>
        /// 查询所有部门类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadDepartmentOutputDto> SelectDeptAll(ReadDepartmentInputDto readDepartmentInputDto);

        /// <summary>
        /// 查询部门类型
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<ReadDepartmentOutputDto> SelectDept(ReadDepartmentInputDto dept);

        /// <summary>
        /// 添加部门类型
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        BaseResponse AddDept(CreateDepartmentInputDto dept);

        /// <summary>
        /// 删除部门类型
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        BaseResponse DelDept(DeleteDepartmentInputDto dept);

        /// <summary>
        /// 更新部门类型
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        BaseResponse UpdDept(UpdateDepartmentInputDto dept);

        #endregion

        #region 客户类型模块

        /// <summary>
        /// 查询所有客户类型(可用)
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeAllCanUse();

        /// <summary>
        /// 查询所有客户类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeAll(ReadCustoTypeInputDto readCustoTypeInputDto);

        /// <summary>
        /// 根据客户类型ID查询类型名称
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        SingleOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeByTypeId(ReadCustoTypeInputDto custoType);

        /// <summary>
        /// 添加客户类型
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        BaseResponse InsertCustoType(CreateCustoTypeInputDto custoType);

        /// <summary>
        /// 删除客户类型
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        BaseResponse DeleteCustoType(DeleteCustoTypeInputDto custoType);

        /// <summary>
        /// 更新客户类型
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        BaseResponse UpdateCustoType(UpdateCustoTypeInputDto custoType);

        #endregion

        #region 证件类型模块

        /// <summary>
        /// 查询所有证件类型(可用)
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeAllCanUse();

        /// <summary>
        /// 查询所有证件类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeAll(ReadPassportTypeInputDto readPassportTypeInputDto);

        /// <summary>
        /// 根据证件类型ID查询类型名称
        /// </summary>
        /// <param name="passPortType"></param>
        /// <returns></returns>
        SingleOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeByTypeId(ReadPassportTypeInputDto passPortType);

        /// <summary>
        /// 添加证件类型
        /// </summary>
        /// <param name="passPortType"></param>
        /// <returns></returns>
        BaseResponse InsertPassPortType(CreatePassportTypeInputDto passPortType);

        /// <summary>
        /// 删除证件类型
        /// </summary>
        /// <param name="portType"></param>
        /// <returns></returns>
        BaseResponse DeletePassPortType(DeletePassportTypeInputDto portType);

        /// <summary>
        /// 更新证件类型
        /// </summary>
        /// <param name="portType"></param>
        /// <returns></returns>
        BaseResponse UpdatePassPortType(UpdatePassportTypeInputDto portType);

        #endregion

        #region 奖惩类型模块

        /// <summary>
        /// 查询所有证件类型(可用)
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeAllCanUse();

        /// <summary>
        /// 查询所有奖惩类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeAll(ReadRewardPunishmentTypeInputDto readRewardPunishmentTypeInputDto);

        /// <summary>
        /// 根据奖惩类型ID查询类型名称
        /// </summary>
        /// <param name="gBType"></param>
        /// <returns></returns>
        SingleOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeByTypeId(ReadRewardPunishmentTypeInputDto gBType);

        /// <summary>
        /// 添加奖惩类型
        /// </summary>
        /// <param name="gBType"></param>
        /// <returns></returns>
        BaseResponse InsertRewardPunishmentType(CreateRewardPunishmentTypeInputDto gBType);

        /// <summary>
        /// 删除奖惩类型
        /// </summary>
        /// <param name="gBType"></param>
        /// <returns></returns>
        BaseResponse DeleteRewardPunishmentType(DeleteRewardPunishmentTypeInputDto gBType);

        /// <summary>
        /// 更新奖惩类型
        /// </summary>
        /// <param name="gBType"></param>
        /// <returns></returns>
        BaseResponse UpdateRewardPunishmentType(UpdateRewardPunishmentTypeInputDto gBType);

        #endregion

        #region 公告类型模块

        /// <summary>
        /// 查询所有公告类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadAppointmentNoticeTypeOutputDto> SelectAppointmentNoticeTypeAll(ReadAppointmentNoticeTypeInputDto readAppointmentNoticeTypeInputDto);

        /// <summary>
        /// 添加公告类型
        /// </summary>
        /// <param name="createAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        BaseResponse CreateAppointmentNoticeType(CreateAppointmentNoticeTypeInputDto createAppointmentNoticeTypeInputDto);

        /// <summary>
        /// 删除公告类型
        /// </summary>
        /// <param name="deleteAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        BaseResponse DeleteAppointmentNoticeType(DeleteAppointmentNoticeTypeInputDto deleteAppointmentNoticeTypeInputDto);

        /// <summary>
        /// 更新公告类型
        /// </summary>
        /// <param name="updateAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdateAppointmentNoticeType(UpdateAppointmentNoticeTypeInputDto updateAppointmentNoticeTypeInputDto);

        #endregion
    }
}
