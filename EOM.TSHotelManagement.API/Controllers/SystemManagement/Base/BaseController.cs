using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 基础信息控制器
    /// </summary>
    public class BaseController : ControllerBase
    {
        private readonly IBaseService baseService;

        public BaseController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        #region 预约类型模块

        /// <summary>
        /// 查询所有预约类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ListOutputDto<EnumDto> SelectReserTypeAll()
        {
            return baseService.SelectReserTypeAll();
        }

        #endregion

        #region 性别模块

        /// <summary>
        /// 查询所有性别类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ListOutputDto<EnumDto> SelectGenderTypeAll()
        {
            return baseService.SelectGenderTypeAll();
        }
        #endregion

        #region 面貌模块

        /// <summary>
        /// 查询所有员工面貌类型
        /// </summary>
        [HttpGet]
        public ListOutputDto<EnumDto> SelectWorkerFeatureAll()
        {
            return baseService.SelectWorkerFeatureAll();
        }

        #endregion

        #region 房间状态模块
        /// <summary>
        /// 获取所有房间状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ListOutputDto<EnumDto> SelectRoomStateAll()
        {
            return baseService.SelectRoomStateAll();
        }
        #endregion

        #region 职位模块

        /// <summary>
        /// 查询职位列表
        /// </summary>
        [RequirePermission("position.view")]
        [HttpGet]
        public ListOutputDto<ReadPositionOutputDto> SelectPositionAll([FromQuery] ReadPositionInputDto position = null)
        {
            return baseService.SelectPositionAll(position);
        }

        /// <summary>
        /// 查询职位详情
        /// </summary>
        [HttpGet]
        public SingleOutputDto<ReadPositionOutputDto> SelectPosition([FromQuery] ReadPositionInputDto position)
        {
            return baseService.SelectPosition(position);
        }

        /// <summary>
        /// 新增职位信息
        /// </summary>
        [RequirePermission("position.create")]
        [HttpPost]
        public BaseResponse AddPosition([FromBody] CreatePositionInputDto position)
        {
            return baseService.AddPosition(position);
        }

        /// <summary>
        /// 删除职位信息
        /// </summary>
        [RequirePermission("position.delete")]
        [HttpPost]
        public BaseResponse DelPosition([FromBody] DeletePositionInputDto position)
        {
            return baseService.DelPosition(position);
        }

        /// <summary>
        /// 更新职位信息
        /// </summary>
        [RequirePermission("position.update")]
        [HttpPost]
        public BaseResponse UpdPosition([FromBody] UpdatePositionInputDto position)
        {
            return baseService.UpdPosition(position);
        }

        #endregion

        #region 民族模块

        /// <summary>
        /// 查询民族列表
        /// </summary>
        [RequirePermission("nation.view")]
        [HttpGet]
        public ListOutputDto<ReadNationOutputDto> SelectNationAll([FromQuery] ReadNationInputDto nation = null)
        {
            return baseService.SelectNationAll(nation);
        }

        /// <summary>
        /// 查询民族详情
        /// </summary>
        [HttpGet]
        public SingleOutputDto<ReadNationOutputDto> SelectNation([FromQuery] ReadNationInputDto nation)
        {
            return baseService.SelectNation(nation);
        }

        /// <summary>
        /// 新增民族信息
        /// </summary>
        [RequirePermission("nation.create")]
        [HttpPost]
        public BaseResponse AddNation([FromBody] CreateNationInputDto nation)
        {
            return baseService.AddNation(nation);
        }

        /// <summary>
        /// 删除民族信息
        /// </summary>
        [RequirePermission("nation.delete")]
        [HttpPost]
        public BaseResponse DelNation([FromBody] DeleteNationInputDto nation)
        {
            return baseService.DelNation(nation);
        }

        /// <summary>
        /// 更新民族信息
        /// </summary>
        [RequirePermission("nation.update")]
        [HttpPost]
        public BaseResponse UpdNation([FromBody] UpdateNationInputDto nation)
        {
            return baseService.UpdNation(nation);
        }

        #endregion

        #region 学历模块

        /// <summary>
        /// 查询学历列表
        /// </summary>
        [RequirePermission("qualification.view")]
        [HttpGet]
        public ListOutputDto<ReadEducationOutputDto> SelectEducationAll([FromQuery] ReadEducationInputDto education = null)
        {
            return baseService.SelectEducationAll(education);
        }

        /// <summary>
        /// 查询学历详情
        /// </summary>
        [HttpGet]
        public SingleOutputDto<ReadEducationOutputDto> SelectEducation([FromQuery] ReadEducationInputDto education)
        {
            return baseService.SelectEducation(education);
        }

        /// <summary>
        /// 新增学历信息
        /// </summary>
        [RequirePermission("qualification.create")]
        [HttpPost]
        public BaseResponse AddEducation([FromBody] CreateEducationInputDto education)
        {
            return baseService.AddEducation(education);
        }

        /// <summary>
        /// 删除学历信息
        /// </summary>
        [RequirePermission("qualification.delete")]
        [HttpPost]
        public BaseResponse DelEducation([FromBody] DeleteEducationInputDto education)
        {
            return baseService.DelEducation(education);
        }

        /// <summary>
        /// 更新学历信息
        /// </summary>
        [RequirePermission("qualification.update")]
        [HttpPost]
        public BaseResponse UpdEducation([FromBody] UpdateEducationInputDto education)
        {
            return baseService.UpdEducation(education);
        }

        #endregion

        #region 部门模块

        /// <summary>
        /// 查询可用部门列表
        /// </summary>
        [RequirePermission("department.view")]
        [HttpGet]
        public ListOutputDto<ReadDepartmentOutputDto> SelectDeptAllCanUse()
        {
            return baseService.SelectDeptAllCanUse();
        }

        /// <summary>
        /// 查询部门列表
        /// </summary>
        [HttpGet]
        public ListOutputDto<ReadDepartmentOutputDto> SelectDeptAll([FromQuery] ReadDepartmentInputDto readDepartmentInputDto)
        {
            return baseService.SelectDeptAll(readDepartmentInputDto);
        }

        /// <summary>
        /// 查询部门详情
        /// </summary>
        [RequirePermission("department.view")]
        [HttpGet]
        public SingleOutputDto<ReadDepartmentOutputDto> SelectDept([FromQuery] ReadDepartmentInputDto dept)
        {
            return baseService.SelectDept(dept);
        }

        /// <summary>
        /// 新增部门信息
        /// </summary>
        [RequirePermission("department.create")]
        [HttpPost]
        public BaseResponse AddDept([FromBody] CreateDepartmentInputDto dept)
        {
            return baseService.AddDept(dept);
        }

        /// <summary>
        /// 删除部门信息
        /// </summary>
        [RequirePermission("department.delete")]
        [HttpPost]
        public BaseResponse DelDept([FromBody] DeleteDepartmentInputDto dept)
        {
            return baseService.DelDept(dept);
        }

        /// <summary>
        /// 更新部门信息
        /// </summary>
        [RequirePermission("department.update")]
        [HttpPost]
        public BaseResponse UpdDept([FromBody] UpdateDepartmentInputDto dept)
        {
            return baseService.UpdDept(dept);
        }

        #endregion

        #region 客户类型模块

        /// <summary>
        /// 查询可用客户类型列表
        /// </summary>
        [RequirePermission("customertype.view")]
        [HttpGet]
        public ListOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeAllCanUse()
        {
            return baseService.SelectCustoTypeAllCanUse();
        }

        /// <summary>
        /// 查询客户类型列表
        /// </summary>
        [HttpGet]
        public ListOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeAll([FromQuery] ReadCustoTypeInputDto readCustoTypeInputDto)
        {
            return baseService.SelectCustoTypeAll(readCustoTypeInputDto);
        }

        /// <summary>
        /// 查询客户类型详情
        /// </summary>
        [RequirePermission("customertype.view")]
        [HttpGet]
        public SingleOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeByTypeId([FromQuery] ReadCustoTypeInputDto custoType)
        {
            return baseService.SelectCustoTypeByTypeId(custoType);
        }

        /// <summary>
        /// 新增客户类型
        /// </summary>
        [RequirePermission("customertype.create")]
        [HttpPost]
        public BaseResponse InsertCustoType([FromBody] CreateCustoTypeInputDto custoType)
        {
            return baseService.InsertCustoType(custoType);
        }

        /// <summary>
        /// 删除客户类型
        /// </summary>
        [RequirePermission("customertype.delete")]
        [HttpPost]
        public BaseResponse DeleteCustoType([FromBody] DeleteCustoTypeInputDto custoType)
        {
            return baseService.DeleteCustoType(custoType);
        }

        /// <summary>
        /// 更新客户类型
        /// </summary>
        [RequirePermission("customertype.update")]
        [HttpPost]
        public BaseResponse UpdateCustoType([FromBody] UpdateCustoTypeInputDto custoType)
        {
            return baseService.UpdateCustoType(custoType);
        }

        #endregion

        #region 证件类型模块

        /// <summary>
        /// 查询可用证件类型列表
        /// </summary>
        [RequirePermission("passport.view")]
        [HttpGet]
        public ListOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeAllCanUse()
        {
            return baseService.SelectPassPortTypeAllCanUse();
        }

        /// <summary>
        /// 查询证件类型列表
        /// </summary>
        [HttpGet]
        public ListOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeAll([FromQuery] ReadPassportTypeInputDto readPassportTypeInputDto)
        {
            return baseService.SelectPassPortTypeAll(readPassportTypeInputDto);
        }

        /// <summary>
        /// 查询证件类型详情
        /// </summary>
        [RequirePermission("passport.view")]
        [HttpGet]
        public SingleOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeByTypeId([FromQuery] ReadPassportTypeInputDto passPortType)
        {
            return baseService.SelectPassPortTypeByTypeId(passPortType);
        }

        /// <summary>
        /// 新增证件类型
        /// </summary>
        [RequirePermission("passport.create")]
        [HttpPost]
        public BaseResponse InsertPassPortType([FromBody] CreatePassportTypeInputDto passPortType)
        {
            return baseService.InsertPassPortType(passPortType);
        }

        /// <summary>
        /// 删除证件类型
        /// </summary>
        [RequirePermission("passport.delete")]
        [HttpPost]
        public BaseResponse DeletePassPortType([FromBody] DeletePassportTypeInputDto portType)
        {
            return baseService.DeletePassPortType(portType);
        }

        /// <summary>
        /// 更新证件类型
        /// </summary>
        [RequirePermission("passport.update")]
        [HttpPost]
        public BaseResponse UpdatePassPortType([FromBody] UpdatePassportTypeInputDto portType)
        {
            return baseService.UpdatePassPortType(portType);
        }

        #endregion

        #region 奖惩类型模块

        /// <summary>
        /// 查询可用奖惩类型列表
        /// </summary>
        [HttpGet]
        public ListOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeAllCanUse()
        {
            return baseService.SelectRewardPunishmentTypeAllCanUse();
        }

        /// <summary>
        /// 查询奖惩类型列表
        /// </summary>
        [HttpGet]
        public ListOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeAll([FromQuery] ReadRewardPunishmentTypeInputDto readRewardPunishmentTypeInputDto)
        {
            return baseService.SelectRewardPunishmentTypeAll(readRewardPunishmentTypeInputDto);
        }

        /// <summary>
        /// 查询奖惩类型详情
        /// </summary>
        [HttpGet]
        public SingleOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeByTypeId([FromQuery] ReadRewardPunishmentTypeInputDto readRewardPunishmentTypeInputDto)
        {
            return baseService.SelectRewardPunishmentTypeByTypeId(readRewardPunishmentTypeInputDto);
        }

        /// <summary>
        /// 新增奖惩类型
        /// </summary>
        [HttpPost]
        public BaseResponse InsertRewardPunishmentType([FromBody] CreateRewardPunishmentTypeInputDto createRewardPunishmentTypeInputDto)
        {
            return baseService.InsertRewardPunishmentType(createRewardPunishmentTypeInputDto);
        }

        /// <summary>
        /// 删除奖惩类型
        /// </summary>
        [HttpPost]
        public BaseResponse DeleteRewardPunishmentType([FromBody] DeleteRewardPunishmentTypeInputDto deleteRewardPunishmentTypeInputDto)
        {
            return baseService.DeleteRewardPunishmentType(deleteRewardPunishmentTypeInputDto);
        }

        /// <summary>
        /// 更新奖惩类型
        /// </summary>
        [HttpPost]
        public BaseResponse UpdateRewardPunishmentType([FromBody] UpdateRewardPunishmentTypeInputDto updateRewardPunishmentTypeInputDto)
        {
            return baseService.UpdateRewardPunishmentType(updateRewardPunishmentTypeInputDto);
        }

        #endregion

        #region 公告类型模块

        /// <summary>
        /// 查询所有公告类型
        /// </summary>
        /// <returns></returns>
        [RequirePermission("noticetype.view")]
        [HttpGet]
        public ListOutputDto<ReadAppointmentNoticeTypeOutputDto> SelectAppointmentNoticeTypeAll([FromQuery] ReadAppointmentNoticeTypeInputDto readAppointmentNoticeTypeInputDto)
        {
            return baseService.SelectAppointmentNoticeTypeAll(readAppointmentNoticeTypeInputDto);
        }

        /// <summary>
        /// 添加公告类型
        /// </summary>
        /// <param name="createAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("noticetype.create")]
        [HttpPost]
        public BaseResponse CreateAppointmentNoticeType([FromBody] CreateAppointmentNoticeTypeInputDto createAppointmentNoticeTypeInputDto)
        {
            return baseService.CreateAppointmentNoticeType(createAppointmentNoticeTypeInputDto);
        }

        /// <summary>
        /// 删除公告类型
        /// </summary>
        /// <param name="deleteAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("noticetype.delete")]
        [HttpPost]
        public BaseResponse DeleteAppointmentNoticeType([FromBody] DeleteAppointmentNoticeTypeInputDto deleteAppointmentNoticeTypeInputDto)
        {
            return baseService.DeleteAppointmentNoticeType(deleteAppointmentNoticeTypeInputDto);
        }

        /// <summary>
        /// 更新公告类型
        /// </summary>
        /// <param name="updateAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        [RequirePermission("noticetype.update")]
        [HttpPost]
        public BaseResponse UpdateAppointmentNoticeType([FromBody] UpdateAppointmentNoticeTypeInputDto updateAppointmentNoticeTypeInputDto)
        {
            return baseService.UpdateAppointmentNoticeType(updateAppointmentNoticeTypeInputDto);
        }

        #endregion
    }
}
