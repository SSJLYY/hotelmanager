using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.CodeLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace EOM.TSHotelManagement.Service
{
    public class SpendService : ISpendService
    {
        private readonly GenericRepository<Spend> spendRepository;
        private readonly GenericRepository<SellThing> sellThingRepository;
        private readonly GenericRepository<Room> roomRepository;
        private readonly GenericRepository<Customer> customerRepository;
        private readonly GenericRepository<CustoType> custoTypeRepository;
        private readonly ILogger<SpendService> logger;

        public SpendService(
            GenericRepository<Spend> spendRepository,
            GenericRepository<SellThing> sellThingRepository,
            GenericRepository<Room> roomRepository,
            GenericRepository<Customer> customerRepository,
            GenericRepository<CustoType> custoTypeRepository,
            ILogger<SpendService> logger)
        {
            this.spendRepository = spendRepository;
            this.sellThingRepository = sellThingRepository;
            this.roomRepository = roomRepository;
            this.customerRepository = customerRepository;
            this.custoTypeRepository = custoTypeRepository;
            this.logger = logger;
        }

        public ListOutputDto<ReadSpendOutputDto> SeletHistorySpendInfoAll(ReadSpendInputDto readSpendInputDto) => BuildSpendList(readSpendInputDto);

        public ListOutputDto<ReadSpendOutputDto> SelectSpendByRoomNo(ReadSpendInputDto readSpendInputDto) => BuildSpendList(readSpendInputDto);

        public ListOutputDto<ReadSpendOutputDto> SelectSpendInfoAll(ReadSpendInputDto readSpendInputDto) => BuildSpendList(readSpendInputDto, nameof(Spend.ConsumptionTime));

        public SingleOutputDto<ReadSpendInputDto> SumConsumptionAmount(ReadSpendInputDto readSpendInputDto)
        {
            readSpendInputDto ??= new ReadSpendInputDto();

            var query = spendRepository.AsQueryable()
                .Where(a => a.IsDelete != 1 && a.CustomerNumber == readSpendInputDto.CustomerNumber && a.SettlementStatus == ConsumptionConstant.UnSettle.Code);

            if (readSpendInputDto.RoomId.HasValue && readSpendInputDto.RoomId.Value > 0)
            {
                query = query.Where(a => a.RoomId == readSpendInputDto.RoomId);
            }

            var totalCountAmount = query.ToList().Sum(a => a.ConsumptionAmount);
            return new SingleOutputDto<ReadSpendInputDto>
            {
                Data = new ReadSpendInputDto
                {
                    RoomId = readSpendInputDto.RoomId,
                    RoomNumber = readSpendInputDto.RoomNumber,
                    CustomerNumber = readSpendInputDto.CustomerNumber,
                    ConsumptionAmount = totalCountAmount
                }
            };
        }

        public BaseResponse UndoCustomerSpend(UndoCustomerSpendInputDto undoCustomerSpendInputDto)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var existingSpend = spendRepository.GetFirst(a => a.Id == undoCustomerSpendInputDto.Id && a.IsDelete != 1);
                if (existingSpend == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, "Spend record not found.");
                }

                if (existingSpend.ConsumptionType != SpendTypeConstant.Product.Code)
                {
                    return new BaseResponse(BusinessStatusCode.BadRequest, "Only product spends can be canceled.");
                }

                if (!string.IsNullOrWhiteSpace(existingSpend.ProductNumber) && existingSpend.ConsumptionQuantity > 0)
                {
                    var product = sellThingRepository.GetFirst(a => a.ProductNumber == existingSpend.ProductNumber && a.IsDelete != 1);
                    if (product == null)
                    {
                        return new BaseResponse(BusinessStatusCode.NotFound, "Product not found.");
                    }

                    product.Stock += existingSpend.ConsumptionQuantity;
                    if (!sellThingRepository.Update(product))
                    {
                        return BaseResponseFactory.ConcurrencyConflict();
                    }
                }

                existingSpend.IsDelete = 1;
                existingSpend.RowVersion = undoCustomerSpendInputDto.RowVersion ?? 0;
                if (!spendRepository.Update(existingSpend))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                scope.Complete();
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Undo customer spend failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, ex.Message);
            }
        }

        public BaseResponse AddCustomerSpend(AddCustomerSpendInputDto addCustomerSpendInputDto)
        {
            if (addCustomerSpendInputDto?.ConsumptionQuantity <= 0 || addCustomerSpendInputDto.ProductPrice <= 0)
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, "Product quantity and price must be greater than zero.");
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var roomResult = RoomReferenceHelper.Resolve(roomRepository, addCustomerSpendInputDto?.RoomId, addCustomerSpendInputDto?.RoomNumber);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, addCustomerSpendInputDto?.RoomId, addCustomerSpendInputDto?.RoomNumber);
                }

                var room = roomResult.Room;
                var customer = customerRepository.GetFirst(a => a.CustomerNumber == room.CustomerNumber && a.IsDelete != 1);
                if (customer == null)
                {
                    return new BaseResponse(BusinessStatusCode.BadRequest, $"Customer '{room.CustomerNumber}' was not found.");
                }

                var customerType = custoTypeRepository.GetFirst(a => a.CustomerType == customer.CustomerType && a.IsDelete != 1);
                var discount = customerType != null && customerType.Discount > 0 && customerType.Discount < 100
                    ? customerType.Discount / 100M
                    : 1M;

                var realAmount = addCustomerSpendInputDto.ProductPrice * addCustomerSpendInputDto.ConsumptionQuantity * discount;
                var existingSpend = spendRepository.GetFirst(a =>
                    a.IsDelete != 1 &&
                    a.SettlementStatus == ConsumptionConstant.UnSettle.Code &&
                    a.ProductNumber == addCustomerSpendInputDto.ProductNumber &&
                    a.RoomId == room.Id);

                if (existingSpend != null)
                {
                    existingSpend.RoomId = room.Id;
                    existingSpend.RoomNumber = room.RoomNumber;
                    existingSpend.ConsumptionType = SpendTypeConstant.Product.Code;
                    existingSpend.ConsumptionQuantity += addCustomerSpendInputDto.ConsumptionQuantity;
                    existingSpend.ConsumptionAmount += realAmount;
                    if (!spendRepository.Update(existingSpend))
                    {
                        return BaseResponseFactory.ConcurrencyConflict();
                    }
                }
                else
                {
                    var newSpend = new Spend
                    {
                        SpendNumber = new UniqueCode().GetNewId("SP-"),
                        RoomId = room.Id,
                        RoomNumber = room.RoomNumber,
                        ProductNumber = addCustomerSpendInputDto.ProductNumber,
                        ProductName = addCustomerSpendInputDto.ProductName,
                        ConsumptionQuantity = addCustomerSpendInputDto.ConsumptionQuantity,
                        CustomerNumber = room.CustomerNumber,
                        ProductPrice = addCustomerSpendInputDto.ProductPrice,
                        ConsumptionAmount = realAmount,
                        ConsumptionTime = DateTime.Now,
                        ConsumptionType = SpendTypeConstant.Product.Code,
                        SettlementStatus = ConsumptionConstant.UnSettle.Code
                    };

                    if (!spendRepository.Insert(newSpend))
                    {
                        return new BaseResponse(BusinessStatusCode.InternalServerError, "Failed to add spend record.");
                    }
                }

                var product = sellThingRepository.GetFirst(a => a.ProductNumber == addCustomerSpendInputDto.ProductNumber && a.IsDelete != 1);
                if (product == null)
                {
                    return new BaseResponse(BusinessStatusCode.BadRequest, $"Product '{addCustomerSpendInputDto.ProductNumber}' was not found.");
                }

                product.Stock -= addCustomerSpendInputDto.ConsumptionQuantity;
                if (!sellThingRepository.Update(product))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                scope.Complete();
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Add customer spend failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, $"Failed to add spend record. {ex.Message}");
            }
        }

        public BaseResponse UpdSpendInfo(UpdateSpendInputDto spend)
        {
            try
            {
                var dbSpend = spendRepository.GetFirst(a => a.SpendNumber == spend.SpendNumber && a.IsDelete != 1);
                if (dbSpend == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, "Spend record not found.");
                }

                Room room = null;
                if (spend.RoomId.HasValue && spend.RoomId.Value > 0)
                {
                    var roomResult = RoomReferenceHelper.Resolve(roomRepository, spend.RoomId, spend.RoomNumber);
                    if (roomResult.Room == null)
                    {
                        return CreateRoomLookupFailure(roomResult, spend.RoomId, spend.RoomNumber);
                    }

                    room = roomResult.Room;
                }
                else if (!string.IsNullOrWhiteSpace(spend.RoomNumber))
                {
                    return new BaseResponse(BusinessStatusCode.BadRequest, "RoomId is required.");
                }

                dbSpend.SettlementStatus = spend.SettlementStatus;
                dbSpend.RoomId = room?.Id ?? dbSpend.RoomId;
                dbSpend.RoomNumber = room?.RoomNumber ?? dbSpend.RoomNumber;
                dbSpend.CustomerNumber = spend.CustomerNumber;
                dbSpend.ProductName = spend.ProductName;
                dbSpend.ConsumptionQuantity = spend.ConsumptionQuantity;
                dbSpend.ProductPrice = spend.ProductPrice;
                dbSpend.ConsumptionAmount = spend.ConsumptionAmount;
                dbSpend.ConsumptionTime = spend.ConsumptionTime;
                dbSpend.ConsumptionType = spend.ConsumptionType;
                dbSpend.RowVersion = spend.RowVersion ?? 0;

                return spendRepository.Update(dbSpend) ? new BaseResponse() : BaseResponseFactory.ConcurrencyConflict();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update spend failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, ex.Message);
            }
        }

        private ListOutputDto<ReadSpendOutputDto> BuildSpendList(ReadSpendInputDto readSpendInputDto, string dateFieldName = null)
        {
            readSpendInputDto ??= new ReadSpendInputDto();
            var filterInput = CreateSpendFilter(readSpendInputDto);

            var where = string.IsNullOrWhiteSpace(dateFieldName)
                ? SqlFilterBuilder.BuildExpression<Spend, ReadSpendInputDto>(filterInput)
                : SqlFilterBuilder.BuildExpression<Spend, ReadSpendInputDto>(filterInput, dateFieldName);

            var query = spendRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            if (readSpendInputDto.RoomId.HasValue && readSpendInputDto.RoomId.Value > 0)
            {
                query = query.Where(a => a.RoomId == readSpendInputDto.RoomId.Value);
            }

            var count = 0;
            List<Spend> spends;
            if (readSpendInputDto.IgnorePaging)
            {
                spends = query.ToList();
                count = spends.Count;
            }
            else
            {
                var page = readSpendInputDto.Page > 0 ? readSpendInputDto.Page : 1;
                var pageSize = readSpendInputDto.PageSize > 0 ? readSpendInputDto.PageSize : 15;
                spends = query.ToPageList(page, pageSize, ref count);
            }

            var rooms = RoomReferenceHelper.LoadRooms(roomRepository, spends.Select(a => a.RoomId), spends.Select(a => a.RoomNumber));
            var result = spends.Select(a => MapSpendToOutput(a, RoomReferenceHelper.FindRoom(rooms, a.RoomId, a.RoomNumber))).ToList();

            result.ForEach(r =>
            {
                r.SettlementStatusDescription = ConsumptionConstant.GetDescriptionByCode(r.SettlementStatus) ?? r.SettlementStatus;
                r.ConsumptionTypeDescription = SpendTypeConstant.GetDescriptionByCode(r.ConsumptionType) ?? r.ConsumptionType;
            });

            return new ListOutputDto<ReadSpendOutputDto>
            {
                Data = new PagedData<ReadSpendOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        private static ReadSpendOutputDto MapSpendToOutput(Spend source, Room room)
        {
            var output = new ReadSpendOutputDto
            {
                Id = source.Id,
                RoomId = source.RoomId ?? room?.Id,
                SpendNumber = source.SpendNumber,
                RoomNumber = room?.RoomNumber ?? source.RoomNumber,
                RoomArea = RoomReferenceHelper.GetRoomArea(room),
                RoomFloor = RoomReferenceHelper.GetRoomFloor(room),
                RoomLocator = RoomReferenceHelper.GetRoomLocator(room),
                CustomerNumber = source.CustomerNumber,
                ProductNumber = source.ProductNumber,
                ProductName = source.ProductName,
                ConsumptionQuantity = source.ConsumptionQuantity,
                ProductPrice = source.ProductPrice,
                ConsumptionAmount = source.ConsumptionAmount,
                ConsumptionTime = source.ConsumptionTime,
                SettlementStatus = source.SettlementStatus,
                ConsumptionType = source.ConsumptionType,
                DataInsUsr = source.DataInsUsr,
                DataInsDate = source.DataInsDate,
                DataChgUsr = source.DataChgUsr,
                DataChgDate = source.DataChgDate,
                RowVersion = source.RowVersion,
                IsDelete = source.IsDelete
            };

            FillSpendDerivedFields(output);
            return output;
        }

        private static void FillSpendDerivedFields(ReadSpendOutputDto item)
        {
            item.SettlementStatusDescription = string.IsNullOrWhiteSpace(item.SettlementStatus)
                ? string.Empty
                : item.SettlementStatus.Equals(ConsumptionConstant.Settled.Code, StringComparison.OrdinalIgnoreCase) ? "Settled" : "Unsettled";

            item.ProductPriceFormatted = item.ProductPrice.ToString("#,##0.00");
            item.ConsumptionAmountFormatted = item.ConsumptionAmount.ToString("#,##0.00");
            item.ConsumptionTypeDescription = item.ConsumptionType == SpendTypeConstant.Product.Code
                ? SpendTypeConstant.Product.Description
                : item.ConsumptionType == SpendTypeConstant.Room.Code
                    ? SpendTypeConstant.Room.Description
                    : SpendTypeConstant.Other.Description;
        }

        private static BaseResponse CreateRoomLookupFailure(RoomResolveResult result, int? roomId, string roomNumber)
        {
            if (!roomId.HasValue || roomId.Value <= 0)
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, "RoomId is required.");
            }

            if (result?.IsAmbiguous == true)
            {
                return new BaseResponse(BusinessStatusCode.Conflict, $"Multiple rooms match room id '{roomId.Value}'.");
            }

            return new BaseResponse(BusinessStatusCode.NotFound, $"RoomId '{roomId.Value}' was not found.");
        }

        private static ReadSpendInputDto CreateSpendFilter(ReadSpendInputDto input)
        {
            return new ReadSpendInputDto
            {
                Page = input.Page,
                PageSize = input.PageSize,
                IgnorePaging = input.IgnorePaging,
                SpendNumber = input.SpendNumber,
                RoomId = null,
                RoomNumber = null,
                CustomerNumber = input.CustomerNumber,
                ProductName = input.ProductName,
                ConsumptionQuantity = input.ConsumptionQuantity,
                ProductPrice = input.ProductPrice,
                ConsumptionAmount = input.ConsumptionAmount,
                SettlementStatus = input.SettlementStatus,
                DateRangeDto = input.DateRangeDto
            };
        }
    }
}
