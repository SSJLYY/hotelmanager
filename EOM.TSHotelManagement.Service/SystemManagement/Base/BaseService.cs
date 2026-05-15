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
using EOM.TSHotelManagement.Common.Helper;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EOM.TSHotelManagement.Service.SystemManagement
{
    /// <summary>
    /// 基础信息接口实现类
    /// </summary>
    public class BaseService(GenericRepository<Domain.Employee> workerRepository, GenericRepository<Education> educationRepository, GenericRepository<Nation> nationRepository, GenericRepository<Department> deptRepository, GenericRepository<Position> positionRepository, GenericRepository<PassportType> passPortTypeRepository, GenericRepository<CustoType> custoTypeRepository, GenericRepository<RewardPunishmentType> goodbadTypeRepository, GenericRepository<AppointmentNoticeType> appointmentNoticeTypeRepository, GenericRepository<Customer> customerRepository, GenericRepository<AppointmentNotice> appointmentNoticeRepository, ILogger<BaseService> logger) : IBaseService
    {

        #region 预约类型模块

        /// <summary>
        /// 查询所有预约类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<EnumDto> SelectReserTypeAll()
        {
            var enumList = Enum.GetValues(typeof(ReserType))
                .Cast<ReserType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();

            return new ListOutputDto<EnumDto>
            {
                Data = new PagedData<EnumDto>
                {
                    Items = enumList,
                    TotalCount = enumList.Count
                }
            };
        }

        #endregion

        #region 性别模块

        /// <summary>
        /// 查询所有性别类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<EnumDto> SelectGenderTypeAll()
        {
            var enumList = Enum.GetValues(typeof(GenderType))
                .Cast<GenderType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();

            return new ListOutputDto<EnumDto>
            {
                Data = new PagedData<EnumDto>
                {
                    Items = enumList,
                    TotalCount = enumList.Count
                }
            };
        }

        #endregion

        #region 面貌模块

        /// <summary>
        /// 查询所有面貌类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<EnumDto> SelectWorkerFeatureAll()
        {
            var enumList = Enum.GetValues(typeof(PoliticalAffiliation))
                .Cast<PoliticalAffiliation>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();

            return new ListOutputDto<EnumDto>
            {
                Data = new PagedData<EnumDto>
                {
                    Items = enumList,
                    TotalCount = enumList.Count
                }
            };
        }

        #endregion

        #region 房间状态模块  
        /// <summary>  
        /// 获取所有房间状态  
        /// </summary>  
        /// <returns></returns>  
        public ListOutputDto<EnumDto> SelectRoomStateAll()
        {
            var enumList = Enum.GetValues(typeof(RoomState))
                .Cast<RoomState>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();

            return new ListOutputDto<EnumDto>
            {
                Data = new PagedData<EnumDto>
                {
                    Items = enumList,
                    TotalCount = enumList.Count
                }
            };
        }
        #endregion

        #region 职位模块

        /// <summary>
        /// 查询所有职位类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadPositionOutputDto> SelectPositionAll(ReadPositionInputDto positionInputDto = null)
        {
            var where = SqlFilterBuilder.BuildExpression<Position, ReadPositionInputDto>(positionInputDto ?? new ReadPositionInputDto());
            var count = 0;
            var positions = new List<Position>();

            if (!positionInputDto.IgnorePaging)
            {
                positions = positionRepository.AsQueryable().Where(where.ToExpression()).ToPageList(positionInputDto.Page, positionInputDto.PageSize, ref count);
            }
            else
            {
                positions = positionRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = positions.Count;
            }

            var result = EntityMapper.MapList<Position, ReadPositionOutputDto>(positions);
            return new ListOutputDto<ReadPositionOutputDto>
            {
                Data = new PagedData<ReadPositionOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询职位类型
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<ReadPositionOutputDto> SelectPosition(ReadPositionInputDto positionInputDto)
        {
            var position = positionRepository.GetFirst(a => a.Id == positionInputDto.PositionId);
            var result = EntityMapper.Map<Position, ReadPositionOutputDto>(position);
            return new SingleOutputDto<ReadPositionOutputDto> { Data = result };
        }

        /// <summary>
        /// 添加职位类型
        /// </summary>
        /// <param name="createPositionInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddPosition(CreatePositionInputDto createPositionInputDto)
        {
            try
            {
                var position = EntityMapper.Map<CreatePositionInputDto, Position>(createPositionInputDto);
                var result = positionRepository.Insert(position);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding position");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Add Position Failure","添加职位类型失败") };
            }
        }

        /// <summary>
        /// 删除职位类型
        /// </summary>
        /// <param name="deletePositionInputDto"></param>
        /// <returns></returns>
        public BaseResponse DelPosition(DeletePositionInputDto deletePositionInputDto)
        {
            try
            {
                if (deletePositionInputDto?.DelIds == null || !deletePositionInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deletePositionInputDto);
                var positions = positionRepository.GetList(a => delIds.Contains(a.Id));

                if (!positions.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Position Not Found", "职位未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deletePositionInputDto, positions, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 当前职位下是否有员工
                var positionNumbers = positions.Select(a => a.PositionNumber).ToList();
                var employeeCount = workerRepository.AsQueryable().Count(a => positionNumbers.Contains(a.Position));
                if (employeeCount > 0)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Position In Use", "职位类型正在被使用，无法删除")
                    };
                }

                var result = positionRepository.SoftDeleteRange(positions);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting positions");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Position Failure", "删除职位类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新职位类型
        /// </summary>
        /// <param name="updatePositionInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdPosition(UpdatePositionInputDto updatePositionInputDto)
        {
            try
            {
                var position = EntityMapper.Map<UpdatePositionInputDto, Position>(updatePositionInputDto);
                var result = positionRepository.Update(position);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error updating position with ID {PositionId}", updatePositionInputDto.PositionId);
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Update Position Failure", "删除职位类型失败") };
            }
        }

        #endregion

        #region 民族模块

        /// <summary>
        /// 查询所有民族类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadNationOutputDto> SelectNationAll(ReadNationInputDto nationInputDto)
        {
            var nations = new List<Nation>();

            var where = SqlFilterBuilder.BuildExpression<Nation, ReadNationInputDto>(nationInputDto ?? new ReadNationInputDto());
            var count = 0;

            if (!nationInputDto.IgnorePaging)
            {
                nations = nationRepository.AsQueryable().Where(where.ToExpression()).ToPageList(nationInputDto.Page, nationInputDto.PageSize, ref count);
            }
            else
            {
                nations = nationRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = nations.Count;
            }

            var result = EntityMapper.MapList<Nation, ReadNationOutputDto>(nations);
            return new ListOutputDto<ReadNationOutputDto>
            {
                Data = new PagedData<ReadNationOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询民族类型
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<ReadNationOutputDto> SelectNation(ReadNationInputDto nationInputDto)
        {
            var nation = nationRepository.GetFirst(a => a.Id == nationInputDto.NationId);
            var result = EntityMapper.Map<Nation, ReadNationOutputDto>(nation);
            return new SingleOutputDto<ReadNationOutputDto> { Data = result };
        }

        /// <summary>
        /// 添加民族类型
        /// </summary>
        /// <param name="createNationInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddNation(CreateNationInputDto createNationInputDto)
        {
            try
            {
                var nation = EntityMapper.Map<CreateNationInputDto, Nation>(createNationInputDto);
                nationRepository.Insert(nation);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding nation");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Add Nation Type Failure","添加民族类型失败") };
            }
        }

        /// <summary>
        /// 删除民族类型
        /// </summary>
        /// <param name="deleteNationInputDto"></param>
        /// <returns></returns>
        public BaseResponse DelNation(DeleteNationInputDto deleteNationInputDto)
        {
            try
            {
                if (deleteNationInputDto?.DelIds == null || !deleteNationInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteNationInputDto);
                var nations = nationRepository.GetList(a => delIds.Contains(a.Id));

                if (!nations.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Nation Not Found", "民族类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteNationInputDto, nations, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 当前民族类型下是否有员工
                var nationNumbers = nations.Select(a => a.NationNumber).ToList();
                var employeeCount = workerRepository.AsQueryable().Count(a => nationNumbers.Contains(a.Ethnicity));
                if (employeeCount > 0)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Nation In Use", "民族类型正在被使用，无法删除")
                    };
                }

                var result = nationRepository.SoftDeleteRange(nations);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting nations");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Nation Type Failure", "删除民族类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新民族类型
        /// </summary>
        /// <param name="updateNationInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdNation(UpdateNationInputDto updateNationInputDto)
        {
            try
            {
                var nation = EntityMapper.Map<UpdateNationInputDto, Nation>(updateNationInputDto);
                var result = nationRepository.Update(nation);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating nation");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Update Nation Type Failure", "更新民族类型失败") };
            }
        }

        #endregion

        #region 学历模块

        /// <summary>
        /// 查询所有学历类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadEducationOutputDto> SelectEducationAll(ReadEducationInputDto educationInputDto = null)
        {
            var where = SqlFilterBuilder.BuildExpression<Education, ReadEducationInputDto>(educationInputDto ?? new ReadEducationInputDto());
            var count = 0;
            var educations = new List<Education>();

            if (!educationInputDto.IgnorePaging)
            {
                educations = educationRepository.AsQueryable().Where(where.ToExpression()).ToPageList(educationInputDto.Page, educationInputDto.PageSize, ref count);
            }
            else
            {
                educations = educationRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = educations.Count;
            }

            var result = EntityMapper.MapList<Education, ReadEducationOutputDto>(educations);
            return new ListOutputDto<ReadEducationOutputDto>
            {
                Data = new PagedData<ReadEducationOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询学历类型
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<ReadEducationOutputDto> SelectEducation(ReadEducationInputDto educationInputDto)
        {
            var education = educationRepository.GetFirst(a => a.EducationNumber == educationInputDto.EducationNumber);
            var result = EntityMapper.Map<Education, ReadEducationOutputDto>(education);
            return new SingleOutputDto<ReadEducationOutputDto> { Data = result };
        }

        /// <summary>
        /// 添加学历类型
        /// </summary>
        /// <param name="createEducationInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddEducation(CreateEducationInputDto createEducationInputDto)
        {
            try
            {
                var education = EntityMapper.Map<CreateEducationInputDto, Education>(createEducationInputDto);
                educationRepository.Insert(education);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding education");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Add Education Type Failure","添加学历类型失败") };
            }
        }

        /// <summary>
        /// 删除学历类型
        /// </summary>
        /// <param name="deleteEducationInputDto"></param>
        /// <returns></returns>
        public BaseResponse DelEducation(DeleteEducationInputDto deleteEducationInputDto)
        {
            try
            {
                if (deleteEducationInputDto?.DelIds == null || !deleteEducationInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }
                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteEducationInputDto);
                var educations = educationRepository.GetList(a => delIds.Contains(a.Id));
                if (!educations.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Education Not Found", "学历类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteEducationInputDto, educations, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 当前学历类型下是否有员工
                var educationNumbers = educations.Select(a => a.EducationNumber).ToList();
                var employeeCount = workerRepository.AsQueryable().Count(a => educationNumbers.Contains(a.EducationLevel));
                if (employeeCount > 0)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Education In Use", "学历类型正在被使用，无法删除")
                    };
                }

                var result = educationRepository.SoftDeleteRange(educations);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting educations");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Education Type Failure", "删除学历类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新学历类型
        /// </summary>
        /// <param name="education"></param>
        /// <returns></returns>
        public BaseResponse UpdEducation(UpdateEducationInputDto education)
        {
            try
            {
                var entity = EntityMapper.Map<UpdateEducationInputDto, Education>(education);
                var result = educationRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating education");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Update Education Type Failure", "更新学历类型失败") };
            }
        }

        #endregion

        #region 部门模块

        /// <summary>
        /// 查询所有部门类型(可用)
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadDepartmentOutputDto> SelectDeptAllCanUse()
        {
            var depts = deptRepository.GetList(a => a.IsDelete != 1);
            var result = EntityMapper.MapList<Department, ReadDepartmentOutputDto>(depts);
            return new ListOutputDto<ReadDepartmentOutputDto>
            {
                Data = new PagedData<ReadDepartmentOutputDto>
                {
                    Items = result,
                    TotalCount = result.Count
                }
            };
        }

        /// <summary>
        /// 查询所有部门类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadDepartmentOutputDto> SelectDeptAll(ReadDepartmentInputDto readDepartmentInputDto)
        {
            var where = SqlFilterBuilder.BuildExpression<Department, ReadDepartmentInputDto>(readDepartmentInputDto ?? new ReadDepartmentInputDto(), nameof(Department.DepartmentCreationDate));

            var count = 0;
            var depts = new List<Department>();

            if (!readDepartmentInputDto.IgnorePaging)
            {
                depts = deptRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readDepartmentInputDto.Page, readDepartmentInputDto.PageSize, ref count);
            }
            else
            {
                depts = deptRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = depts.Count;
            }

            var parentDepartmentNumbers = depts.Where(a => !a.ParentDepartmentNumber.IsNullOrEmpty()).Select(a => a.ParentDepartmentNumber).ToList();
            var parentDepartments = deptRepository.GetList(a => parentDepartmentNumbers.Contains(a.DepartmentNumber)).ToDictionary(d => d.DepartmentNumber);
            var departmentLeaderNumbers = depts.Where(a => !a.DepartmentLeader.IsNullOrEmpty()).Select(a => a.DepartmentLeader).ToList();
            var departmentLeaders = workerRepository.GetList(a => departmentLeaderNumbers.Contains(a.EmployeeId)).ToDictionary(e => e.EmployeeId);

            depts.ForEach(source =>
            {
                if (!string.IsNullOrWhiteSpace(source.ParentDepartmentNumber) && parentDepartments.TryGetValue(source.ParentDepartmentNumber, out var parentDepartment))
                {
                    source.ParentDepartmentName = parentDepartment.DepartmentName;
                }
                else
                {
                    source.ParentDepartmentName = "";
                }

                if (!string.IsNullOrWhiteSpace(source.DepartmentLeader) && departmentLeaders.TryGetValue(source.DepartmentLeader, out var departmentLeader))
                {
                    source.LeaderName = departmentLeader.Name;
                }
                else
                {
                    source.LeaderName = "";
                }
            });
            var result = EntityMapper.MapList<Department, ReadDepartmentOutputDto>(depts);
            return new ListOutputDto<ReadDepartmentOutputDto>
            {
                Data = new PagedData<ReadDepartmentOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询部门类型
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<ReadDepartmentOutputDto> SelectDept(ReadDepartmentInputDto dept)
        {
            var department = deptRepository.GetFirst(a => a.Id == dept.Id);
            var result = EntityMapper.Map<Department, ReadDepartmentOutputDto>(department);
            return new SingleOutputDto<ReadDepartmentOutputDto> { Data = result };
        }

        /// <summary>
        /// 添加部门类型
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public BaseResponse AddDept(CreateDepartmentInputDto dept)
        {
            try
            {
                var department = EntityMapper.Map<CreateDepartmentInputDto, Department>(dept);
                deptRepository.Insert(department);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding department");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Add Department Failure","添加部门类型失败") };
            }
        }

        /// <summary>
        /// 删除部门类型
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public BaseResponse DelDept(DeleteDepartmentInputDto dept)
        {
            try
            {
                if (dept?.DelIds == null || !dept.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }
                var delIds = DeleteConcurrencyHelper.GetDeleteIds(dept);
                var departments = deptRepository.GetList(a => delIds.Contains(a.Id));
                if (!departments.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Department Not Found", "部门类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(dept, departments, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 当前部门类型下是否有员工
                var departmentNumbers = departments.Select(a => a.DepartmentNumber).ToList();
                var employeeCount = workerRepository.AsQueryable().Count(a => departmentNumbers.Contains(a.Department));
                if (employeeCount > 0) 
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Department In Use", "部门类型正在被使用，无法删除")
                    };
                }

                var result = deptRepository.SoftDeleteRange(departments);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting departments");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Department Failure", "删除部门类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新部门类型
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public BaseResponse UpdDept(UpdateDepartmentInputDto dept)
        {
            try
            {
                var department = EntityMapper.Map<UpdateDepartmentInputDto, Department>(dept);
                var result = deptRepository.Update(department);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating department");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Update Department Failure", "更新部门类型失败") };
            }
        }

        #endregion

        #region 客户类型模块

        /// <summary>
        /// 查询所有客户类型(可用)
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeAllCanUse()
        {
            var custoTypes = custoTypeRepository.GetList(a => a.IsDelete != 1);
            var result = EntityMapper.MapList<CustoType, ReadCustoTypeOutputDto>(custoTypes);
            return new ListOutputDto<ReadCustoTypeOutputDto>
            {
                Data = new PagedData<ReadCustoTypeOutputDto>
                {
                    Items = result,
                    TotalCount = result.Count
                }
            };
        }

        /// <summary>
        /// 查询所有客户类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeAll(ReadCustoTypeInputDto readCustoTypeInputDto)
        {
            var where = SqlFilterBuilder.BuildExpression<CustoType, ReadCustoTypeInputDto>(readCustoTypeInputDto);
            var count = 0;
            var custoTypes = new List<CustoType>();

            if (!readCustoTypeInputDto.IgnorePaging)
            {
                custoTypes = custoTypeRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readCustoTypeInputDto.Page, readCustoTypeInputDto.PageSize, ref count);
            }
            else
            {
                custoTypes = custoTypeRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = custoTypes.Count;
            }

            var result = EntityMapper.MapList<CustoType, ReadCustoTypeOutputDto>(custoTypes);
            return new ListOutputDto<ReadCustoTypeOutputDto>
            {
                Data = new PagedData<ReadCustoTypeOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 根据客户类型ID查询类型名称
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadCustoTypeOutputDto> SelectCustoTypeByTypeId(ReadCustoTypeInputDto custoType)
        {
            var custoTypeEntity = custoTypeRepository.GetFirst(a => a.CustomerType == custoType.CustomerType && a.IsDelete != 1);
            var result = EntityMapper.Map<CustoType, ReadCustoTypeOutputDto>(custoTypeEntity);
            return new SingleOutputDto<ReadCustoTypeOutputDto> { Data = result };
        }

        /// <summary>
        /// 添加客户类型
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        public BaseResponse InsertCustoType(CreateCustoTypeInputDto custoType)
        {
            try
            {
                var custoTypeEntity = EntityMapper.Map<CreateCustoTypeInputDto, CustoType>(custoType);
                custoTypeRepository.Insert(custoTypeEntity);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding customer type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Add Customer Type Failure", "添加客户类型失败") };
            }
        }

        /// <summary>
        /// 删除客户类型
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        public BaseResponse DeleteCustoType(DeleteCustoTypeInputDto custoType)
        {
            try
            {
                if (custoType?.DelIds == null || !custoType.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }
                var delIds = DeleteConcurrencyHelper.GetDeleteIds(custoType);
                var custoTypes = custoTypeRepository.GetList(a => delIds.Contains(a.Id));
                if (!custoTypes.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Customer Type Not Found", "客户类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(custoType, custoTypes, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 当前客户类型下是否有客户
                var customerTypeNumbers = custoTypes.Select(a => a.CustomerType).ToList();
                var customerCount = customerRepository.AsQueryable().Count(a => customerTypeNumbers.Contains(a.CustomerType));
                if (customerCount > 0)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Customer Type In Use", "客户类型正在被使用，无法删除")
                    };
                }

                var result = custoTypeRepository.SoftDeleteRange(custoTypes);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting departments");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Customer Type Failure", "删除客户类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新客户类型
        /// </summary>
        /// <param name="custoType"></param>
        /// <returns></returns>
        public BaseResponse UpdateCustoType(UpdateCustoTypeInputDto custoType)
        {
            try
            {
                var entity = EntityMapper.Map<UpdateCustoTypeInputDto, CustoType>(custoType);
                var result = custoTypeRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating customer type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Update Customer Type Failure", "更新客户类型失败") };
            }
        }

        #endregion

        #region 证件类型模块

        /// <summary>
        /// 查询所有证件类型(可用)
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeAllCanUse()
        {
            var passPortTypes = passPortTypeRepository.GetList(a => a.IsDelete != 1);
            var result = EntityMapper.MapList<PassportType, ReadPassportTypeOutputDto>(passPortTypes);
            return new ListOutputDto<ReadPassportTypeOutputDto>
            {
                Data = new PagedData<ReadPassportTypeOutputDto>
                {
                    Items = result,
                    TotalCount = result.Count
                }
            };
        }

        /// <summary>
        /// 查询所有证件类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeAll(ReadPassportTypeInputDto readPassportTypeInputDto)
        {
            var where = SqlFilterBuilder.BuildExpression<PassportType, ReadPassportTypeInputDto>(readPassportTypeInputDto);
            var count = 0;
            var passPortTypes = new List<PassportType>();

            if (!readPassportTypeInputDto.IgnorePaging)
            {
                passPortTypes = passPortTypeRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readPassportTypeInputDto.Page, readPassportTypeInputDto.PageSize, ref count);
            }
            else
            {
                passPortTypes = passPortTypeRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = passPortTypes.Count;
            }

            var result = EntityMapper.MapList<PassportType, ReadPassportTypeOutputDto>(passPortTypes);
            return new ListOutputDto<ReadPassportTypeOutputDto>
            {
                Data = new PagedData<ReadPassportTypeOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 根据证件类型ID查询类型名称
        /// </summary>
        /// <param name="passPortType"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadPassportTypeOutputDto> SelectPassPortTypeByTypeId(ReadPassportTypeInputDto passPortType)
        {
            var passPortTypeEntity = passPortTypeRepository.GetFirst(a => a.PassportId == passPortType.PassportId && a.IsDelete != 1);
            var result = EntityMapper.Map<PassportType, ReadPassportTypeOutputDto>(passPortTypeEntity);
            return new SingleOutputDto<ReadPassportTypeOutputDto> { Data = result };
        }

        /// <summary>
        /// 添加证件类型
        /// </summary>
        /// <param name="passPortType"></param>
        /// <returns></returns>
        public BaseResponse InsertPassPortType(CreatePassportTypeInputDto passPortType)
        {
            try
            {
                var passPortTypeEntity = EntityMapper.Map<CreatePassportTypeInputDto, PassportType>(passPortType);
                passPortTypeRepository.Insert(passPortTypeEntity);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding passport type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Add Passport Type Failure", "添加证件类型失败") };
            }
        }

        /// <summary>
        /// 删除证件类型
        /// </summary>
        /// <param name="portType"></param>
        /// <returns></returns>
        public BaseResponse DeletePassPortType(DeletePassportTypeInputDto portType)
        {
            try
            {
                if (portType?.DelIds == null || !portType.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }
                var delIds = DeleteConcurrencyHelper.GetDeleteIds(portType);
                var passPortTypes = passPortTypeRepository.GetList(a => delIds.Contains(a.Id));
                if (!passPortTypes.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Passport Type Not Found", "证件类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(portType, passPortTypes, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 当前证件类型下是否有客户
                var passportTypeNumbers = passPortTypes.Select(a => a.PassportId).ToList();
                var customerCount = customerRepository.AsQueryable().Count(a => passportTypeNumbers.Contains(a.IdCardType));
                if (customerCount > 0)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Passport Type In Use", "证件类型正在被使用，无法删除")
                    };
                }

                // 当前证件类型下是否有员工
                var employeeCount = workerRepository.AsQueryable().Count(a => passportTypeNumbers.Contains(a.IdCardType));
                if (employeeCount > 0)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Passport Type In Use", "证件类型正在被使用，无法删除")
                    };
                }

                var result = passPortTypeRepository.SoftDeleteRange(passPortTypes);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting passport types");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Passport Type Failure", "删除证件类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新证件类型
        /// </summary>
        /// <param name="portType"></param>
        /// <returns></returns>
        public BaseResponse UpdatePassPortType(UpdatePassportTypeInputDto portType)
        {
            try
            {
                var entity = EntityMapper.Map<UpdatePassportTypeInputDto, PassportType>(portType);
                var result = passPortTypeRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating passport type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Update Passport Type Failure", "更新证件类型失败") };
            }
        }

        #endregion

        #region 奖惩类型模块

        /// <summary>
        /// 查询所有奖惩类型(可用)
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeAllCanUse()
        {
            var gBTypes = goodbadTypeRepository.GetList(a => a.IsDelete != 1);
            var result = EntityMapper.MapList<RewardPunishmentType, ReadRewardPunishmentTypeOutputDto>(gBTypes);
            return new ListOutputDto<ReadRewardPunishmentTypeOutputDto>
            {
                Data = new PagedData<ReadRewardPunishmentTypeOutputDto>
                {
                    Items = result,
                    TotalCount = result.Count
                }
            };
        }

        /// <summary>
        /// 查询所有奖惩类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeAll(ReadRewardPunishmentTypeInputDto readRewardPunishmentTypeInputDto)
        {
            var where = SqlFilterBuilder.BuildExpression<RewardPunishmentType, ReadRewardPunishmentTypeInputDto>(readRewardPunishmentTypeInputDto);
            var count = 0;
            var gBTypes = new List<RewardPunishmentType>();

            if (!readRewardPunishmentTypeInputDto.IgnorePaging)
            {
                gBTypes = goodbadTypeRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readRewardPunishmentTypeInputDto.Page, readRewardPunishmentTypeInputDto.PageSize, ref count);
            }
            else
            {
                gBTypes = goodbadTypeRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = gBTypes.Count;
            }

            var result = EntityMapper.MapList<RewardPunishmentType, ReadRewardPunishmentTypeOutputDto>(gBTypes);
            return new ListOutputDto<ReadRewardPunishmentTypeOutputDto>
            {
                Data = new PagedData<ReadRewardPunishmentTypeOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 根据奖惩类型ID查询类型名称
        /// </summary>
        /// <param name="gBType"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadRewardPunishmentTypeOutputDto> SelectRewardPunishmentTypeByTypeId(ReadRewardPunishmentTypeInputDto gBType)
        {
            var gBTypeEntity = goodbadTypeRepository.GetFirst(a => a.RewardPunishmentTypeId == gBType.RewardPunishmentTypeId && a.IsDelete != 1);
            var result = EntityMapper.Map<RewardPunishmentType, ReadRewardPunishmentTypeOutputDto>(gBTypeEntity);
            return new SingleOutputDto<ReadRewardPunishmentTypeOutputDto> { Data = result };
        }

        /// <summary>
        /// 添加奖惩类型
        /// </summary>
        /// <param name="gBType"></param>
        /// <returns></returns>
        public BaseResponse InsertRewardPunishmentType(CreateRewardPunishmentTypeInputDto request)
        {
            try
            {
                var rewardPunishmentType = EntityMapper.Map<CreateRewardPunishmentTypeInputDto, RewardPunishmentType>(request);
                var result = goodbadTypeRepository.Insert(rewardPunishmentType);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding Reward & Punishment type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Add Reward & Punishment Type Failure", "添加奖惩类型失败") };
            }
        }

        /// <summary>
        /// 删除奖惩类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseResponse DeleteRewardPunishmentType(DeleteRewardPunishmentTypeInputDto request)
        {
            try
            {
                if (request?.DelIds == null || !request.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }
                var delIds = DeleteConcurrencyHelper.GetDeleteIds(request);
                var rewardPunishmentTypes = goodbadTypeRepository.GetList(a => delIds.Contains(a.Id));
                if (!rewardPunishmentTypes.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Reward & Punishment Type Not Found", "奖惩类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(request, rewardPunishmentTypes, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var result = goodbadTypeRepository.SoftDeleteRange(rewardPunishmentTypes);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting Reward & Punishment types");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Reward & Punishment Type Failure", "删除奖惩类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新奖惩类型
        /// </summary>
        /// <param name="gBType"></param>
        /// <returns></returns>
        public BaseResponse UpdateRewardPunishmentType(UpdateRewardPunishmentTypeInputDto gBType)
        {
            try
            {
                var entity = EntityMapper.Map<UpdateRewardPunishmentTypeInputDto, RewardPunishmentType>(gBType);
                var result = goodbadTypeRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating Reward & Punishment type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("Update Reward & Punishment Type Failure", "更新奖惩类型失败") };
            }
        }

        #endregion

        #region 公告类型模块

        /// <summary>
        /// 查询所有公告类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadAppointmentNoticeTypeOutputDto> SelectAppointmentNoticeTypeAll(ReadAppointmentNoticeTypeInputDto readAppointmentNoticeTypeInputDto)
        {
            var Data = new List<AppointmentNoticeType>();

            var where = SqlFilterBuilder.BuildExpression<AppointmentNoticeType, ReadAppointmentNoticeTypeInputDto>(readAppointmentNoticeTypeInputDto);

            var count = 0;

            if (!readAppointmentNoticeTypeInputDto.IgnorePaging)
            {
                Data = appointmentNoticeTypeRepository.AsQueryable().Where(where.ToExpression())
                    .ToPageList(readAppointmentNoticeTypeInputDto.Page, readAppointmentNoticeTypeInputDto.PageSize, ref count);
            }
            else
            {
                Data = appointmentNoticeTypeRepository.AsQueryable().Where(where.ToExpression()).ToList();
            }

            var result = EntityMapper.MapList<AppointmentNoticeType, ReadAppointmentNoticeTypeOutputDto>(Data);

            return new ListOutputDto<ReadAppointmentNoticeTypeOutputDto>
            {
                Data = new PagedData<ReadAppointmentNoticeTypeOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 添加公告类型
        /// </summary>
        /// <param name="createAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        public BaseResponse CreateAppointmentNoticeType(CreateAppointmentNoticeTypeInputDto createAppointmentNoticeTypeInputDto)
        {
            try
            {
                if (appointmentNoticeTypeRepository.IsAny(a => a.NoticeTypeNumber == createAppointmentNoticeTypeInputDto.NoticeTypeNumber))
                {
                    return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("appointment notice number already exits.", "公告类型编号已存在") };
                }
                var entity = EntityMapper.Map<CreateAppointmentNoticeTypeInputDto, AppointmentNoticeType>(createAppointmentNoticeTypeInputDto);
                var result = appointmentNoticeTypeRepository.Insert(entity);
                if (!result)
                {
                    logger.LogError(LocalizationHelper.GetLocalizedString("insert appointment notice failed.", "公告类型添加失败"));
                    return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("insert appointment notice failed.", "公告类型添加失败") };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting appointment notice type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("insert appointment notice failed.", "公告类型添加失败") };
            }

            return new BaseResponse { Code = BusinessStatusCode.Success, Message = LocalizationHelper.GetLocalizedString("insert appointment notice successful.", "公告类型添加成功") };
        }

        /// <summary>
        /// 删除公告类型
        /// </summary>
        /// <param name="deleteAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        public BaseResponse DeleteAppointmentNoticeType(DeleteAppointmentNoticeTypeInputDto deleteAppointmentNoticeTypeInputDto)
        {
            try
            {
                if (deleteAppointmentNoticeTypeInputDto?.DelIds == null || !deleteAppointmentNoticeTypeInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }
                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteAppointmentNoticeTypeInputDto);
                var appointmentNoticeTypes = appointmentNoticeTypeRepository.GetList(a => delIds.Contains(a.Id));
                if (!appointmentNoticeTypes.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Appointment Notice Type Not Found", "公告类型未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteAppointmentNoticeTypeInputDto, appointmentNoticeTypes, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 当前公告类型下是否有公告
                var noticeTypeNumbers = appointmentNoticeTypes.Select(a => a.NoticeTypeNumber).ToList();
                var appointmentNoticeCount = appointmentNoticeRepository.AsQueryable().Count(a => noticeTypeNumbers.Contains(a.NoticeType));
                if (appointmentNoticeCount > 0)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Appointment Notice Type In Use", "公告类型正在被使用，无法删除")
                    };
                }


                var result = appointmentNoticeTypeRepository.SoftDeleteRange(appointmentNoticeTypes);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting appointment notice types");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Appointment Notice Type Failure", "删除公告类型失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新公告类型
        /// </summary>
        /// <param name="updateAppointmentNoticeTypeInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdateAppointmentNoticeType(UpdateAppointmentNoticeTypeInputDto updateAppointmentNoticeTypeInputDto)
        {
            try
            {
                if (!appointmentNoticeTypeRepository.IsAny(a => a.NoticeTypeNumber == updateAppointmentNoticeTypeInputDto.NoticeTypeNumber))
                {
                    return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("appointment notice number does not already.", "公告类型编号不存在") };
                }
                var entity = EntityMapper.Map<UpdateAppointmentNoticeTypeInputDto, AppointmentNoticeType>(updateAppointmentNoticeTypeInputDto);
                var result = appointmentNoticeTypeRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                return new BaseResponse { Code = BusinessStatusCode.Success, Message = LocalizationHelper.GetLocalizedString("update appointment notice successful.", "公告类型更新成功") };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating appointment notice type");
                return new BaseResponse { Code = BusinessStatusCode.InternalServerError, Message = LocalizationHelper.GetLocalizedString("update appointment notice failed.", "公告类型更新失败") };
            }
        }

        #endregion
    }
}

