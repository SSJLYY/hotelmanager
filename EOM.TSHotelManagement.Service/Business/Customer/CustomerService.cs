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

namespace EOM.TSHotelManagement.Service.Business.Customer
{
    /// <summary>
    /// 客户信息接口实现类
    /// </summary>
    public class CustomerService(GenericRepository<Domain.Customer> custoRepository, GenericRepository<Domain.Room> roomRepository, GenericRepository<Spend> spendRepository, GenericRepository<PassportType> passPortTypeRepository, GenericRepository<CustoType> custoTypeRepository, GenericRepository<Role> roleRepository, GenericRepository<UserRole> userRoleRepository, DataProtectionHelper dataProtector, ILogger<CustomerService> logger) : ICustomerService
    {

        /// <summary>
        /// 添加客户信息
        /// </summary>
        /// <param name="custo"></param>
        public BaseResponse InsertCustomerInfo(CreateCustomerInputDto custo)
        {
            string NewID = dataProtector.EncryptCustomerData(custo.IdCardNumber);
            string NewTel = dataProtector.EncryptCustomerData(custo.PhoneNumber);
            custo.IdCardNumber = NewID;
            custo.PhoneNumber = NewTel;
            try
            {
                if (custoRepository.IsAny(a => a.CustomerNumber == custo.CustomerNumber))
                {
                    return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString("customer number already exist.", "客户编号已存在"), Code = BusinessStatusCode.InternalServerError };
                }
                var customer = EntityMapper.Map<CreateCustomerInputDto, Domain.Customer>(custo);
                var result = custoRepository.Insert(customer);
                if (!result)
                {
                    logger.LogError(LocalizationHelper.GetLocalizedString("Insert Customer Failed", "客户信息添加失败"));
                    return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Insert Customer Failed", "客户信息添加失败"));
                }

                // 将客户加入“客户组”角色，便于与管理员一样进行权限配置
                const string customerRoleNumber = "R-CUSTOMER";

                // 确保客户组角色存在
                if (!roleRepository.AsQueryable().Any(r => r.RoleNumber == customerRoleNumber && r.IsDelete != 1))
                {
                    roleRepository.Insert(new Role
                    {
                        RoleNumber = customerRoleNumber,
                        RoleName = LocalizationHelper.GetLocalizedString("Customer Group", "客户组"),
                        RoleDescription = LocalizationHelper.GetLocalizedString("Unified permission group for customers", "客户统一权限组"),
                        IsDelete = 0,
                        DataInsUsr = customer.DataInsUsr,
                        DataInsDate = DateTime.Now
                    });
                }

                // 绑定客户到客户组角色
                if (!userRoleRepository.AsQueryable().Any(ur => ur.UserNumber == customer.CustomerNumber && ur.RoleNumber == customerRoleNumber))
                {
                    userRoleRepository.Insert(new UserRole
                    {
                        UserNumber = customer.CustomerNumber,
                        RoleNumber = customerRoleNumber,
                        DataInsUsr = customer.DataInsUsr,
                        DataInsDate = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting customer information for customer number {CustomerNumber}", custo.CustomerNumber);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Insert Customer Failed", "客户信息添加失败"));
            }

            return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Insert Customer Success", "客户信息添加成功"));
        }

        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <param name="custo"></param>
        /// <returns></returns>
        public BaseResponse UpdCustomerInfo(UpdateCustomerInputDto custo)
        {
            string NewID = dataProtector.EncryptCustomerData(custo.IdCardNumber);
            string NewTel = dataProtector.EncryptCustomerData(custo.PhoneNumber);
            custo.IdCardNumber = NewID;
            custo.PhoneNumber = NewTel;
            try
            {
                if (!custoRepository.IsAny(a => a.CustomerNumber == custo.CustomerNumber))
                {
                    return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString("customer number does not exist.", "客户编号不存在"), Code = BusinessStatusCode.InternalServerError };
                }
                var customer = EntityMapper.Map<UpdateCustomerInputDto, Domain.Customer>(custo);
                var result = custoRepository.Update(customer);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating customer information for customer number {CustomerNumber}", custo.CustomerNumber);
                return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="custo"></param>
        /// <returns></returns>
        public BaseResponse DelCustomerInfo(DeleteCustomerInputDto custo)
        {
            try
            {
                if (custo?.DelIds == null || !custo.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(custo);
                var customers = custoRepository.GetList(a => delIds.Contains(a.Id));

                if (!customers.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Customer Information Not Found", "客户信息未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(custo, customers, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var customerNumbers = customers.Select(c => c.CustomerNumber).ToList();
                var occupiedState = Convert.ToInt32(RoomState.Occupied);

                var occupiedCustomer = roomRepository.GetFirst(a => customerNumbers.Contains(a.CustomerNumber) && a.RoomStateId == occupiedState);
                if (occupiedCustomer != null)
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError,
                        string.Format(LocalizationHelper.GetLocalizedString("Customer {0} is currently occupying a room", "客户{0}当前正在占用房间"), occupiedCustomer.CustomerNumber));
                }

                var unsettledCustomer = spendRepository.GetFirst(a => customerNumbers.Contains(a.CustomerNumber) && a.SettlementStatus == ConsumptionConstant.UnSettle.Code);
                if (unsettledCustomer != null)
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError,
                        string.Format(LocalizationHelper.GetLocalizedString("Customer {0} has unsettled bills", "客户{0}有未结算的账单"), unsettledCustomer.CustomerNumber));
                }

                // 批量软删除
                custoRepository.SoftDeleteRange(customers);

                return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Delete Customer Success", "客户信息删除成功"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting customer information for customer IDs {CustomerIds}", string.Join(", ", custo.DelIds.Select(x => x.Id)));
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Delete Customer Failed", "客户信息删除失败"));
            }
        }

        /// <summary>
        /// 更新客户类型(即会员等级)
        /// </summary>
        /// <param name="updateCustomerInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdCustomerTypeByCustoNo(UpdateCustomerInputDto updateCustomerInputDto)
        {
            try
            {
                var customer = custoRepository.GetFirst(a => a.CustomerNumber == updateCustomerInputDto.CustomerNumber && a.IsDelete != 1);
                if (customer == null)
                {
                    return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString("customer number does not exist.", "客户编号不存在"), Code = BusinessStatusCode.InternalServerError };
                }
                customer.CustomerType = updateCustomerInputDto.CustomerType;
                customer.RowVersion = updateCustomerInputDto.RowVersion ?? 0;
                var result = custoRepository.Update(customer);

                if (result)
                {
                    return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Update Customer Type Success", "客户类型更新成功"));
                }
                else
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating customer type for customer number {CustomerNumber}", updateCustomerInputDto.CustomerNumber);
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Update Customer Type Failed", "客户类型更新失败"));
            }
        }

        /// <summary>
        /// 查询所有客户信息
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadCustomerOutputDto> SelectCustomers(ReadCustomerInputDto readCustomerInputDto)
        {
            readCustomerInputDto ??= new ReadCustomerInputDto();

            var where = SqlFilterBuilder.BuildExpression<Domain.Customer, ReadCustomerInputDto>(readCustomerInputDto, nameof(Domain.Customer.DateOfBirth));
            var query = custoRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            query = query.OrderBy(a => a.CustomerNumber);

            var count = 0;
            List<Domain.Customer> custos;
            if (!readCustomerInputDto.IgnorePaging)
            {
                var page = readCustomerInputDto.Page > 0 ? readCustomerInputDto.Page : 1;
                var pageSize = readCustomerInputDto.PageSize > 0 ? readCustomerInputDto.PageSize : 15;
                custos = query.ToPageList(page, pageSize, ref count);
            }
            else
            {
                custos = query.ToList();
                count = custos.Count;
            }

            var passPortTypeMap = passPortTypeRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.PassportId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.PassportName ?? "");

            var custoTypeMap = custoTypeRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.CustomerType)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.CustomerTypeName ?? "");

            var genderMap = Enum.GetValues(typeof(GenderType))
                .Cast<GenderType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToDictionary(x => x.Id, x => x.Description ?? "");

            List<ReadCustomerOutputDto> customerOutputDtos;
            var useParallelProjection = readCustomerInputDto.IgnorePaging && custos.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadCustomerOutputDto[custos.Count];
                System.Threading.Tasks.Parallel.For(0, custos.Count, i =>
                {
                    dtoArray[i] = MapToCustomerOutputDto(custos[i], genderMap, custoTypeMap, passPortTypeMap);
                });
                customerOutputDtos = dtoArray.ToList();
            }
            else
            {
                customerOutputDtos = custos.Select(source => MapToCustomerOutputDto(source, genderMap, custoTypeMap, passPortTypeMap)).ToList();
            }

            return new ListOutputDto<ReadCustomerOutputDto>
            {
                Data = new PagedData<ReadCustomerOutputDto>
                {
                    Items = customerOutputDtos,
                    TotalCount = count
                }
            };
        }

        private ReadCustomerOutputDto MapToCustomerOutputDto(Domain.Customer source, Dictionary<int, string> genderMap, Dictionary<int, string> custoTypeMap, Dictionary<int, string> passPortTypeMap)
        {
            return new ReadCustomerOutputDto
            {
                Id = source.Id,
                CustomerNumber = source.CustomerNumber,
                Name = source.Name,
                Gender = source.Gender,
                IdCardType = source.IdCardType,
                GenderName = genderMap.TryGetValue(source.Gender, out var genderName) ? genderName : "",
                PhoneNumber = dataProtector.SafeDecryptCustomerData(source.PhoneNumber),
                DateOfBirth = source.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                CustomerType = source.CustomerType,
                CustomerTypeName = custoTypeMap.TryGetValue(source.CustomerType, out var customerTypeName) ? customerTypeName : "",
                PassportName = passPortTypeMap.TryGetValue(source.IdCardType, out var passportName) ? passportName : "",
                IdCardNumber = dataProtector.SafeDecryptCustomerData(source.IdCardNumber),
                Address = source.Address ?? "",
                DataInsUsr = source.DataInsUsr,
                DataInsDate = source.DataInsDate,
                DataChgUsr = source.DataChgUsr,
                DataChgDate = source.DataChgDate,
                RowVersion = source.RowVersion,
                IsDelete = source.IsDelete
            };
        }

        /// <summary>
        /// 查询指定客户信息
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<ReadCustomerOutputDto> SelectCustoByInfo(ReadCustomerInputDto custo)
        {
            //查询出所有性别类型
            var genderMap = Enum.GetValues(typeof(GenderType))
                .Cast<GenderType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToDictionary(x => x.Id, x => x.Description ?? "");
            //查询出所有证件类型
            var passPortTypeMap = passPortTypeRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.PassportId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.PassportName ?? "");
            //查询出所有客户类型
            var custoTypeMap = custoTypeRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.CustomerType)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.CustomerTypeName ?? "");
            //查询出所有客户信息
            SingleOutputDto<ReadCustomerOutputDto> singleOutputDto = new SingleOutputDto<ReadCustomerOutputDto>();

            var where = SqlFilterBuilder.BuildExpression<Domain.Customer, ReadCustomerInputDto>(custo);

            var customer = custoRepository.AsQueryable().Where(where.ToExpression()).Single();

            if (customer == null)
            {
                return new SingleOutputDto<ReadCustomerOutputDto> { Code = BusinessStatusCode.NotFound, Message = "该用户不存在" };
            }

            singleOutputDto.Data = EntityMapper.Map<Domain.Customer, ReadCustomerOutputDto>(customer);

            //解密身份证号码/联系方式（失败时回退原值）
            singleOutputDto.Data.IdCardNumber = dataProtector.SafeDecryptCustomerData(customer.IdCardNumber);
            singleOutputDto.Data.PhoneNumber = dataProtector.SafeDecryptCustomerData(customer.PhoneNumber);
            //性别类型
            singleOutputDto.Data.GenderName = genderMap.TryGetValue((int)customer.Gender!, out var genderName) ? genderName : "";
            //证件类型
            singleOutputDto.Data.PassportName = passPortTypeMap.TryGetValue(customer.IdCardType, out var passportName) ? passportName : "";
            //客户类型
            singleOutputDto.Data.CustomerTypeName = custoTypeMap.TryGetValue(customer.CustomerType, out var customerTypeName) ? customerTypeName : "";

            return singleOutputDto;
        }

    }
}
