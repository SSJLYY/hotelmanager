using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Common.Helper;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using System.Text;

namespace EOM.TSHotelManagement.Service.Dashboard
{
    public class DashboardService(GenericRepository<Room> roomRepository, GenericRepository<RoomType> roomTypeRepository, GenericRepository<Reser> reserRepository, GenericRepository<Customer> customerRepository, GenericRepository<CustoType> custoTypeRepository, GenericRepository<Spend> spendRepository, GenericRepository<SellThing> sellThingRepository, GenericRepository<Domain.Employee> employeeRepository, GenericRepository<Department> departmentRepository, GenericRepository<EmployeeCheck> employeeCheckRepository, DataProtectionHelper dataProtector) : IDashboardService
    {

        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<RoomStatisticsOutputDto> RoomStatistics()
        {
            var roomStatisticsOutputDto = new RoomStatisticsOutputDto();

            try
            {
                var roomStates = Enum.GetValues(typeof(RoomState))
                    .Cast<RoomState>()
                    .Select(e => new EnumDto
                    {
                        Id = (int)e,
                        Name = e.ToString(),
                        Description = EnumHelper.GetEnumDescription(e)
                    })
                    .ToList();
                var roomTypes = roomTypeRepository.AsQueryable().Where(a => a.IsDelete != 1).ToList();

                var resers = reserRepository.AsQueryable()
                            .Where(a => a.ReservationStatus == 0).ToList();

                var roomStateData = roomRepository.AsQueryable().Where(a => roomStates.Select(b => b.Id).ToList().Contains(a.RoomStateId)).ToList();

                var roomTypeIds = roomTypes.Select(a => a.RoomTypeId).ToList();
                var roomTypeData = roomRepository.AsQueryable().Where(a => roomTypeIds.Contains(a.RoomTypeId) && a.IsDelete != 1).ToList();

                roomStatisticsOutputDto.Status = new TempRoomStatus
                {
                    Vacant = roomStateData.Count(a => a.RoomStateId == (int)RoomState.Vacant),
                    Occupied = roomStateData.Count(a => a.RoomStateId == (int)RoomState.Occupied),
                    Maintenance = roomStateData.Count(a => a.RoomStateId == (int)RoomState.Maintenance),
                    Dirty = roomStateData.Count(a => a.RoomStateId == (int)RoomState.Dirty),
                    Reserved = roomStateData.Count(a => a.RoomStateId == (int)RoomState.Reserved)
                };

                var roomTypeDict = roomTypes.ToDictionary(rt => rt.RoomTypeId, rt => rt.RoomTypeName);
                roomStatisticsOutputDto.Types = roomTypes.GroupBy(a => a.RoomTypeId)
                    .Select(g => new TempRoomType
                    {
                        Name = roomTypeDict.GetValueOrDefault(g.Key, "未知房型"),
                        Total = roomTypeData.Count(a => a.RoomTypeId == g.Key),
                        Remaining = roomTypeData.Count(a => a.RoomTypeId == g.Key && a.RoomStateId == (int)RoomState.Vacant)
                    }).ToList();

                roomStatisticsOutputDto.ReservationAlerts = roomStateData
                       .Where(a => a.RoomStateId == (int)RoomState.Reserved)
                       .Select(a =>
                       {
                           var reservation = resers.SingleOrDefault(b => b.RoomId.HasValue && b.RoomId.Value == a.Id);
                           var roomType = roomTypes.SingleOrDefault(b => b.RoomTypeId == a.RoomTypeId);
                           return new TempReservationAlert
                           {
                               RoomType = roomType.RoomTypeName,
                               GuestName = reservation?.CustomerName,
                               GuestPhoneNo = reservation != null && !reservation.ReservationPhoneNumber.IsNullOrEmpty()
                               ? dataProtector.SafeDecryptReserData(reservation.ReservationPhoneNumber)
                               : string.Empty,
                               EndDate = reservation?.ReservationEndDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue
                           };
                       }).ToList();
            }
            catch (Exception ex)
            {
                roomStatisticsOutputDto.Status = new TempRoomStatus
                {
                    Vacant = 0,
                    Occupied = 0,
                    Maintenance = 0,
                    Dirty = 0,
                    Reserved = 0
                };
                roomStatisticsOutputDto.Types = new List<TempRoomType>();
                roomStatisticsOutputDto.ReservationAlerts = new List<TempReservationAlert>();
                return new SingleOutputDto<RoomStatisticsOutputDto>
                {
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message),
                    Code = BusinessStatusCode.InternalServerError
                };
            }

            return new SingleOutputDto<RoomStatisticsOutputDto>
            {
                Data = roomStatisticsOutputDto
            };
        }

        /// <summary>
        /// 获取业务统计信息
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<BusinessStatisticsOutputDto> BusinessStatistics()
        {
            var businessStatisticsOutputDto = new BusinessStatisticsOutputDto();

            var nowUtc = DateTime.UtcNow;
            var today = nowUtc.Date;
            var weekStart = today.AddDays(-6);
            var yearStart = today.AddDays(-365);

            try
            {
                var customerTypes = custoTypeRepository.AsQueryable().Where(a => a.IsDelete != 1).ToList();
                var customers = customerRepository.AsQueryable().Where(a => a.IsDelete != 1).ToList();
                var startDate = today.AddYears(-1);
                var allSpends = spendRepository.AsQueryable()
                    .Where(a => a.ConsumptionTime >= startDate && a.IsDelete != 1)
                    .ToList();

                businessStatisticsOutputDto.GenderRatio = new TempGenderRatio
                {
                    Male = customers.Count(a => a.Gender == (int)GenderType.Male),
                    Female = customers.Count(a => a.Gender == (int)GenderType.Female)
                };

                var memberTypeDict = customerTypes.ToDictionary(rt => rt.CustomerType, rt => rt.CustomerTypeName);
                businessStatisticsOutputDto.MemberTypes = customerTypes
                    .GroupBy(a => a.CustomerType)
                    .Select(g => new TempMemberType
                    {
                        Type = customerTypes.Any(ct => ct.CustomerType == g.Key)
                               ? memberTypeDict[g.Key]
                               : "未知类型",
                        Count = customers.Count(a => a.CustomerType == g.Key)
                    })
                    .ToList();

                businessStatisticsOutputDto.DailyConsumption = new TempDailyConsumption
                {
                    Total = allSpends.Where(a => a.ConsumptionTime.Date == today)
                    .Sum(a => a.ConsumptionAmount),
                    Settled = allSpends.Where(a => a.ConsumptionTime.Date == today &&
                                                 a.SettlementStatus == ConsumptionConstant.Settled.Code)
                      .Sum(a => a.ConsumptionAmount)
                };

                businessStatisticsOutputDto.WeeklyConsumption = new TempDailyConsumption
                {
                    Total = allSpends.Where(a => a.ConsumptionTime.Date >= weekStart)
                    .Sum(a => a.ConsumptionAmount),
                    Settled = allSpends.Where(a => a.ConsumptionTime.Date >= weekStart &&
                                                 a.SettlementStatus == ConsumptionConstant.Settled.Code)
                      .Sum(a => a.ConsumptionAmount)
                };

                businessStatisticsOutputDto.YearConsumption = new TempDailyConsumption
                {
                    Total = allSpends.Where(a => a.ConsumptionTime.Date >= yearStart)
                    .Sum(a => a.ConsumptionAmount),
                    Settled = allSpends.Where(a => a.ConsumptionTime.Date >= yearStart &&
                                                 a.SettlementStatus == ConsumptionConstant.Settled.Code)
                      .Sum(a => a.ConsumptionAmount)
                };

                businessStatisticsOutputDto.TotalConsumption = new TempDailyConsumption
                {
                    Total = allSpends.Sum(a => a.ConsumptionAmount),
                    Settled = allSpends.Where(a => a.SettlementStatus == ConsumptionConstant.Settled.Code)
                      .Sum(a => a.ConsumptionAmount)
                };
            }
            catch (Exception ex)
            {
                return new SingleOutputDto<BusinessStatisticsOutputDto>
                {
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message),
                    Code = BusinessStatusCode.InternalServerError
                };
            }

            return new SingleOutputDto<BusinessStatisticsOutputDto>
            {
                Data = businessStatisticsOutputDto
            };
        }

        /// <summary>
        /// 获取后勤统计信息
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<LogisticsDataOutputDto> LogisticsStatistics()
        {
            var dto = new LogisticsDataOutputDto();
            try
            {
                var sellThings = sellThingRepository.AsQueryable()
                    .Where(a => a.IsDelete != 1)
                    .ToList();

                var TotalCount = sellThings.Count;
                if (TotalCount == 0)
                {
                    dto.TotalProducts = 0;
                    dto.InventoryWarning = new TempInventoryWarning
                    {
                        Percent = 0,
                        Status = "success",
                        Text = LocalizationHelper.GetLocalizedString("No products", "暂无商品")
                    };
                    return new SingleOutputDto<LogisticsDataOutputDto>
                    {
                        Data = dto
                    };
                }

                var dangerProducts = sellThings
                    .Where(a => a.Stock <= 50)
                    .Select(a => a.ProductName)
                    .ToList();

                var warningProducts = sellThings
                    .Where(a => a.Stock > 50 && a.Stock <= 100)
                    .Select(a => a.ProductName)
                    .ToList();

                var statusBuilder = new StringBuilder();
                var lowStockList = new List<string>();

                if (dangerProducts.Any())
                {
                    dto.InventoryWarning = new TempInventoryWarning
                    {
                        Status = "error",
                        Percent = (int)Math.Round((dangerProducts.Count * 100.0) / TotalCount),
                        Text = LocalizationHelper.GetLocalizedString(
                            $"{dangerProducts.Count} products in critical stock",
                            $"{dangerProducts.Count}种商品库存告急"),
                        LowStockProducts = dangerProducts
                    };
                }
                else if (warningProducts.Any())
                {
                    dto.InventoryWarning = new TempInventoryWarning
                    {
                        Status = "warning",
                        Percent = (int)Math.Round((warningProducts.Count * 100.0) / TotalCount),
                        Text = LocalizationHelper.GetLocalizedString(
                            $"{warningProducts.Count} products in low stock",
                            $"{warningProducts.Count}种商品库存预警"),
                        LowStockProducts = warningProducts
                    };
                }
                else
                {
                    dto.InventoryWarning = new TempInventoryWarning
                    {
                        Status = "success",
                        Percent = 0,
                        Text = LocalizationHelper.GetLocalizedString("Stock normal", "库存正常"),
                        LowStockProducts = new List<string>()
                    };
                }

                dto.TotalProducts = (int)sellThings.Sum(a => a.Stock);
                dto.RecentRecords = spendRepository.AsQueryable()
                    .Where(a => a.IsDelete != 1 && a.ConsumptionType == SpendTypeConstant.Product.Code)
                    .OrderByDescending(a => a.ConsumptionTime)
                    .Take(3)
                    .Select(a => new TempInventoryRecord
                    {
                        RecordId = a.SpendNumber,
                        OperationType = a.ConsumptionType == SpendTypeConstant.Product.Code || a.ConsumptionType == SpendTypeConstant.Other.Code ? TempInventoryOperationType.Outbound
                        : TempInventoryOperationType.Inbound,
                        ProductName = a.ProductName,
                        Quantity = a.ConsumptionQuantity
                    }).ToList();

            }
            catch (Exception)
            {
                return new SingleOutputDto<LogisticsDataOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString(
                    "System error, please try again later",
                    "系统繁忙，请稍后重试")
                };

            }
            return new SingleOutputDto<LogisticsDataOutputDto>
            {
                Data = dto
            };
        }

        /// <summary>
        /// 获取人事统计信息
        /// </summary>
        /// <returns></returns>
        public SingleOutputDto<HumanResourcesOutputDto> HumanResourcesStatistics()
        {
            var humanResourcesOutputDto = new HumanResourcesOutputDto();

            try
            {
                var employees = employeeRepository.AsQueryable().Where(a => a.IsDelete != 1).ToList();
                var departments = departmentRepository.AsQueryable().Where(a => a.IsDelete != 1).ToList();
                var employeeIds = employees.Select(a => a.EmployeeId).ToList();

                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var todayChecks = employeeCheckRepository.AsQueryable()
                    .Where(a => employeeIds.Contains(a.EmployeeId)
                             && a.IsDelete != 1
                             && a.CheckTime >= today && a.CheckTime < tomorrow)
                    .ToList();

                humanResourcesOutputDto.TotalEmployees = employees.Count;
                humanResourcesOutputDto.TotalDepartments = departments.Count;

                var morningChecks = todayChecks
                    .Where(a => a.CheckStatus == 0)
                    .GroupBy(a => a.EmployeeId)
                    .Select(g => g.First())
                    .ToList();

                var eveningChecks = todayChecks
                    .Where(a => a.CheckStatus == 1)
                    .GroupBy(a => a.EmployeeId)
                    .Select(g => g.First())
                    .ToList();

                var absentEmployees = employees
                    .Where(e => !morningChecks.Any(m => m.EmployeeId == e.EmployeeId)
                             && !eveningChecks.Any(e => e.EmployeeId == e.EmployeeId))
                    .ToList();

                humanResourcesOutputDto.Attendance = new TempAttendanceRecord
                {
                    MorningPresent = morningChecks.Count,
                    EveningPresent = eveningChecks.Count,
                    Absent = absentEmployees.Count,
                    Late = morningChecks.Count(a => a.CheckTime.Hour > 9)
                };
            }
            catch (Exception ex)
            {
                return new SingleOutputDto<HumanResourcesOutputDto>
                {
                    Data = humanResourcesOutputDto,
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message),
                    Code = BusinessStatusCode.InternalServerError
                };
            }

            return new SingleOutputDto<HumanResourcesOutputDto>
            {
                Data = humanResourcesOutputDto
            };
        }
    }
}
