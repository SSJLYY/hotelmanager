using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Common.Helper;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.CodeLib;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace EOM.TSHotelManagement.Service.Business.Room
{
    public class RoomService : IRoomService
    {
        private readonly GenericRepository<Domain.Room> roomRepository;
        private readonly GenericRepository<Spend> spendRepository;
        private readonly GenericRepository<RoomType> roomTypeRepository;
        private readonly GenericRepository<EnergyManagement> energyRepository;
        private readonly GenericRepository<Domain.Customer> custoRepository;
        private readonly GenericRepository<CustoType> custoTypeRepository;
        private readonly GenericRepository<VipLevelRule> vipLevelRuleRepository;
        private readonly GenericRepository<Domain.Reser> reserRepository;
        private readonly UniqueCode uniqueCode;
        private readonly ILogger<RoomService> logger;

        public RoomService(
            GenericRepository<Domain.Room> roomRepository,
            GenericRepository<Spend> spendRepository,
            GenericRepository<RoomType> roomTypeRepository,
            GenericRepository<EnergyManagement> energyRepository,
            GenericRepository<Domain.Customer> custoRepository,
            GenericRepository<CustoType> custoTypeRepository,
            GenericRepository<VipLevelRule> vipLevelRuleRepository,
            GenericRepository<Domain.Reser> reserRepository,
            UniqueCode uniqueCode,
            ILogger<RoomService> logger)
        {
            this.roomRepository = roomRepository;
            this.spendRepository = spendRepository;
            this.roomTypeRepository = roomTypeRepository;
            this.energyRepository = energyRepository;
            this.custoRepository = custoRepository;
            this.custoTypeRepository = custoTypeRepository;
            this.vipLevelRuleRepository = vipLevelRuleRepository;
            this.reserRepository = reserRepository;
            this.uniqueCode = uniqueCode;
            this.logger = logger;
        }

        public ListOutputDto<ReadRoomOutputDto> SelectRoomByRoomState(ReadRoomInputDto readRoomInputDto) => BuildRoomList(readRoomInputDto);

        public ListOutputDto<ReadRoomOutputDto> SelectCanUseRoomAll() => BuildRoomList(new ReadRoomInputDto
        {
            IgnorePaging = true,
            RoomStateId = (int)RoomState.Vacant
        });

        public ListOutputDto<ReadRoomOutputDto> SelectRoomAll(ReadRoomInputDto readRoomInputDto) => BuildRoomList(readRoomInputDto);

        public ListOutputDto<ReadRoomOutputDto> SelectRoomByTypeName(ReadRoomInputDto readRoomInputDto) => BuildRoomList(readRoomInputDto);

        public SingleOutputDto<ReadRoomOutputDto> SelectRoomByRoomNo(ReadRoomInputDto readRoomInputDto)
        {
            var roomResult = ResolveRoom(readRoomInputDto);
            if (roomResult.Room == null)
            {
                return CreateRoomLookupFailureOutput(roomResult, readRoomInputDto?.RoomNumber, readRoomInputDto?.RoomArea, readRoomInputDto?.RoomFloor);
            }

            NormalizeRoom(roomResult.Room);
            var data = MapRoomToOutput(
                roomResult.Room,
                BuildRoomTypeMap(),
                BuildCustomerMap(new List<Domain.Room> { roomResult.Room }),
                BuildRoomStateMap());

            return new SingleOutputDto<ReadRoomOutputDto> { Data = data };
        }

        public SingleOutputDto<ReadRoomOutputDto> DayByRoomNo(ReadRoomInputDto roomInputDto)
        {
            var roomResult = ResolveRoom(roomInputDto);
            if (roomResult.Room == null)
            {
                return CreateRoomLookupFailureOutput(roomResult, roomInputDto?.RoomNumber, roomInputDto?.RoomArea, roomInputDto?.RoomFloor);
            }

            var lastCheckInTime = NormalizeStayDateTime(roomResult.Room.LastCheckInTime);
            if (lastCheckInTime.HasValue)
            {
                var days = CalculateStayDays(lastCheckInTime.Value, DateTime.Now);
                return new SingleOutputDto<ReadRoomOutputDto> { Data = new ReadRoomOutputDto { StayDays = days } };
            }

            return new SingleOutputDto<ReadRoomOutputDto> { Data = new ReadRoomOutputDto { StayDays = 0 } };
        }

        public BaseResponse UpdateRoomInfo(UpdateRoomInputDto r)
        {
            return UpdateRoomCore(r, true);
        }

        public BaseResponse UpdateRoomInfoWithReser(UpdateRoomInputDto r)
        {
            return UpdateRoomCore(r, false);
        }

        private BaseResponse UpdateRoomCore(UpdateRoomInputDto r, bool isFullUpdate)
        {
            try
            {
                var roomResult = ResolveRoom(r);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, r?.RoomNumber, r?.RoomArea, r?.RoomFloor);
                }

                var room = roomResult.Room;
                room.RoomStateId = r.RoomStateId;

                if (isFullUpdate)
                {
                    room.CustomerNumber = r.CustomerNumber;
                    room.LastCheckInTime = NormalizeStayDateTime(r.LastCheckInTime);
                    var pricingResponse = ApplyPricingSelection(room, r.PricingCode);
                    if (pricingResponse != null)
                    {
                        return pricingResponse;
                    }
                }

                room.DataChgDate = r.DataChgDate;
                room.DataChgUsr = r.DataChgUsr;
                room.RowVersion = r.RowVersion ?? 0;

                return roomRepository.Update(room) ? new BaseResponse() : BaseResponseFactory.ConcurrencyConflict();
            }
            catch (Exception ex)
            {
                var updateType = isFullUpdate ? "full" : "with reservation";
                logger.LogError(ex, "Error updating room info ({UpdateType}) for room number {RoomNumber}", updateType, r.RoomNumber);
                return ErrorResponse(ex);
            }
        }

        public SingleOutputDto<ReadRoomOutputDto> SelectCanUseRoomAllByRoomState() => CountByState(RoomState.Vacant, count => new ReadRoomOutputDto { Vacant = count });

        public SingleOutputDto<ReadRoomOutputDto> SelectNotUseRoomAllByRoomState() => CountByState(RoomState.Occupied, count => new ReadRoomOutputDto { Occupied = count });

        public object SelectRoomByRoomPrice(ReadRoomInputDto readRoomInputDto)
        {
            var roomResult = ResolveRoom(readRoomInputDto);
            if (roomResult.Room == null)
            {
                return 0M;
            }

            if (string.IsNullOrWhiteSpace(readRoomInputDto?.PricingCode))
            {
                return GetEffectiveRoomRent(roomResult.Room);
            }

            var roomType = roomTypeRepository.GetFirst(a => a.RoomTypeId == roomResult.Room.RoomTypeId && a.IsDelete != 1);
            if (roomType == null)
            {
                return 0M;
            }

            var pricingItem = RoomPricingHelper.ResolvePricingItem(roomType.RoomRent, roomType.RoomDeposit, roomType.PricingItemsJson, readRoomInputDto.PricingCode);
            return pricingItem?.RoomRent ?? 0M;
        }

        public SingleOutputDto<ReadRoomPricingOutputDto> SelectRoomPricingOptions(ReadRoomInputDto readRoomInputDto)
        {
            var roomResult = ResolveRoom(readRoomInputDto);
            Domain.Room room = null;
            RoomType roomType = null;

            if (roomResult.Room != null)
            {
                room = roomResult.Room;
                roomType = roomTypeRepository.GetFirst(a => a.RoomTypeId == room.RoomTypeId && a.IsDelete != 1);
            }
            else if (readRoomInputDto?.RoomTypeId > 0)
            {
                roomType = roomTypeRepository.GetFirst(a => a.RoomTypeId == readRoomInputDto.RoomTypeId && a.IsDelete != 1);
            }

            if (roomType == null)
            {
                if (roomResult.IsAmbiguous)
                {
                    return new SingleOutputDto<ReadRoomPricingOutputDto>
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = "Multiple rooms match the current room number.",
                        Data = new ReadRoomPricingOutputDto { PricingItems = new List<RoomTypePricingItemDto>() }
                    };
                }

                return new SingleOutputDto<ReadRoomPricingOutputDto>
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = "Room type pricing not found.",
                    Data = new ReadRoomPricingOutputDto { PricingItems = new List<RoomTypePricingItemDto>() }
                };
            }

            var pricingItems = RoomPricingHelper.BuildPricingItems(roomType.RoomRent, roomType.RoomDeposit, roomType.PricingItemsJson);
            if (room != null
                && !string.IsNullOrWhiteSpace(room.RoomPricingCode)
                && pricingItems.All(a => !string.Equals(a.PricingCode, room.RoomPricingCode, StringComparison.OrdinalIgnoreCase)))
            {
                pricingItems.Add(new RoomTypePricingItemDto
                {
                    PricingCode = room.RoomPricingCode,
                    PricingName = string.IsNullOrWhiteSpace(room.RoomPricingName) ? room.RoomPricingCode : room.RoomPricingName,
                    RoomRent = room.AppliedRoomRent,
                    RoomDeposit = room.AppliedRoomDeposit,
                    StayHours = room.PricingStayHours,
                    IsDefault = false
                });
            }

            var pricingEvaluation = room == null
                ? new RoomPricingEvaluation
                {
                    SelectedPricingCode = RoomPricingHelper.DefaultPricingCode,
                    SelectedPricingName = RoomPricingHelper.DefaultPricingName,
                    EffectivePricingCode = RoomPricingHelper.DefaultPricingCode,
                    EffectivePricingName = RoomPricingHelper.DefaultPricingName,
                    EffectiveRoomRent = roomType.RoomRent,
                    EffectiveRoomDeposit = roomType.RoomDeposit
                }
                : EvaluateRoomPricing(room);

            return new SingleOutputDto<ReadRoomPricingOutputDto>
            {
                Data = new ReadRoomPricingOutputDto
                {
                    RoomId = room?.Id,
                    RoomNumber = room?.RoomNumber ?? string.Empty,
                    RoomLocator = room == null ? string.Empty : RoomLocatorHelper.BuildLocator(room.RoomArea, room.RoomFloor, room.RoomNumber),
                    RoomTypeId = roomType.RoomTypeId,
                    RoomTypeName = roomType.RoomTypeName,
                    CurrentPricingCode = pricingEvaluation.SelectedPricingCode,
                    CurrentPricingName = pricingEvaluation.SelectedPricingName,
                    PricingStayHours = pricingEvaluation.PricingStayHours,
                    IsPricingTimedOut = pricingEvaluation.IsPricingTimedOut,
                    EffectivePricingCode = pricingEvaluation.EffectivePricingCode,
                    EffectivePricingName = pricingEvaluation.EffectivePricingName,
                    LastCheckInTime = NormalizeStayDateTime(room?.LastCheckInTime),
                    EffectiveRoomRent = pricingEvaluation.EffectiveRoomRent,
                    EffectiveRoomDeposit = pricingEvaluation.EffectiveRoomDeposit,
                    PricingItems = pricingItems.OrderBy(a => a.Sort).ThenBy(a => a.PricingCode).ToList()
                }
            };
        }

        public SingleOutputDto<ReadRoomOutputDto> SelectNotClearRoomAllByRoomState() => CountByState(RoomState.Dirty, count => new ReadRoomOutputDto { Dirty = count });

        public SingleOutputDto<ReadRoomOutputDto> SelectFixingRoomAllByRoomState() => CountByState(RoomState.Maintenance, count => new ReadRoomOutputDto { Maintenance = count });

        public SingleOutputDto<ReadRoomOutputDto> SelectReservedRoomAllByRoomState() => CountByState(RoomState.Reserved, count => new ReadRoomOutputDto { Reserved = count });

        public BaseResponse UpdateRoomStateByRoomNo(UpdateRoomInputDto updateRoomInputDto)
        {
            try
            {
                var roomResult = ResolveRoom(updateRoomInputDto);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, updateRoomInputDto?.RoomNumber, updateRoomInputDto?.RoomArea, updateRoomInputDto?.RoomFloor);
                }

                roomResult.Room.RoomStateId = updateRoomInputDto.RoomStateId;
                roomResult.Room.RowVersion = updateRoomInputDto.RowVersion ?? 0;
                return roomRepository.Update(roomResult.Room) ? new BaseResponse() : BaseResponseFactory.ConcurrencyConflict();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        public BaseResponse InsertRoom(CreateRoomInputDto rn)
        {
            try
            {
                var normalizedRoomNumber = rn.RoomNumber?.Trim();
                var normalizedRoomArea = RoomLocatorHelper.NormalizeArea(rn.RoomArea);
                var duplicateExists = roomRepository.AsQueryable()
                    .Where(a => a.IsDelete != 1 && a.RoomNumber == normalizedRoomNumber && a.RoomFloor == rn.RoomFloor)
                    .ToList()
                    .Any(a => string.Equals(RoomLocatorHelper.NormalizeArea(a.RoomArea), normalizedRoomArea, StringComparison.OrdinalIgnoreCase));

                if (duplicateExists)
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("This room already exists.", "房间已存在"), Code = BusinessStatusCode.InternalServerError };
                }

                var entity = EntityMapper.Map<CreateRoomInputDto, Domain.Room>(rn);
                entity.RoomNumber = normalizedRoomNumber ?? string.Empty;
                entity.RoomArea = string.IsNullOrWhiteSpace(normalizedRoomArea) ? string.Empty : normalizedRoomArea;
                entity.LastCheckInTime = null;
                entity.LastCheckOutTime = null;
                entity.AppliedRoomRent = 0M;
                entity.AppliedRoomDeposit = 0M;
                entity.RoomPricingCode = string.Empty;
                entity.RoomPricingName = string.Empty;
                entity.PricingStayHours = null;
                entity.PricingStartTime = null;
                NormalizeRoom(entity);
                roomRepository.Insert(entity);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting room with room number {RoomNumber}", rn.RoomNumber);
                return ErrorResponse(ex);
            }
        }

        public BaseResponse UpdateRoom(UpdateRoomInputDto rn)
        {
            try
            {
                var roomResult = ResolveRoom(rn);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, rn?.RoomNumber, rn?.RoomArea, rn?.RoomFloor);
                }

                var room = roomResult.Room;
                var normalizedRoomNumber = rn.RoomNumber?.Trim();
                var normalizedRoomArea = RoomLocatorHelper.NormalizeArea(rn.RoomArea);
                var duplicateExists = roomRepository.AsQueryable()
                    .Where(a => a.IsDelete != 1 && a.Id != room.Id && a.RoomNumber == normalizedRoomNumber && a.RoomFloor == rn.RoomFloor)
                    .ToList()
                    .Any(a => string.Equals(RoomLocatorHelper.NormalizeArea(a.RoomArea), normalizedRoomArea, StringComparison.OrdinalIgnoreCase));

                if (duplicateExists)
                {
                    return new BaseResponse { Message = "This room already exists.", Code = BusinessStatusCode.InternalServerError };
                }

                room.RoomNumber = normalizedRoomNumber;
                room.RoomArea = string.IsNullOrWhiteSpace(normalizedRoomArea) ? null : normalizedRoomArea;
                room.RoomFloor = rn.RoomFloor;
                room.RoomTypeId = rn.RoomTypeId;
                room.CustomerNumber = rn.CustomerNumber;
                room.LastCheckInTime = NormalizeStayDateTime(rn.LastCheckInTime);
                room.LastCheckOutTime = NormalizeStayDateTime(rn.LastCheckOutTime);
                room.RoomStateId = rn.RoomStateId;
                room.RoomRent = rn.RoomRent;
                room.RoomDeposit = rn.RoomDeposit;
                room.RoomLocation = rn.RoomLocation;
                room.RowVersion = rn.RowVersion ?? 0;
                room.DataChgUsr = rn.DataChgUsr;
                room.DataChgDate = rn.DataChgDate;
                room.IsDelete = rn.IsDelete;
                NormalizeRoom(room);

                return roomRepository.Update(room) ? new BaseResponse() : BaseResponseFactory.ConcurrencyConflict();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating room with room number {RoomNumber}", rn.RoomNumber);
                return ErrorResponse(ex);
            }
        }

        public BaseResponse DeleteRoom(DeleteRoomInputDto rn)
        {
            try
            {
                if (rn?.DelIds == null || !rn.DelIds.Any())
                {
                    return new BaseResponse { Code = BusinessStatusCode.BadRequest, Message = "Parameters invalid" };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(rn);
                var rooms = roomRepository.GetList(a => delIds.Contains(a.Id));
                if (!rooms.Any())
                {
                    return new BaseResponse { Code = BusinessStatusCode.NotFound, Message = "Room information not found" };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(rn, rooms, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var roomIds = rooms.Select(a => a.Id).ToList();
                var hasReservation = reserRepository.IsAny(a =>
                    a.IsDelete != 1 &&
                    a.ReservationEndDate >= DateOnly.FromDateTime(DateTime.Today) &&
                    a.RoomId.HasValue &&
                    roomIds.Contains(a.RoomId.Value));
                if (hasReservation)
                {
                    return new BaseResponse { Code = BusinessStatusCode.Conflict, Message = "Cannot delete rooms with active reservations" };
                }

                roomRepository.SoftDeleteRange(rooms);
                return new BaseResponse(BusinessStatusCode.Success, "Delete room success");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        public BaseResponse TransferRoom(TransferRoomDto transferRoomDto)
        {
            try
            {
                if (transferRoomDto == null
                    || !transferRoomDto.OriginalRoomId.HasValue
                    || transferRoomDto.OriginalRoomId.Value <= 0
                    || !transferRoomDto.TargetRoomId.HasValue
                    || transferRoomDto.TargetRoomId.Value <= 0)
                {
                    return new BaseResponse { Message = "OriginalRoomId and TargetRoomId are required", Code = BusinessStatusCode.BadRequest };
                }

                using var scope = CreateTransactionScope();

                var customer = custoRepository.GetFirst(a => a.CustomerNumber == transferRoomDto.CustomerNumber && a.IsDelete != 1);
                if (customer.IsNullOrEmpty())
                {
                    return new BaseResponse { Message = "The customer does not exist", Code = BusinessStatusCode.InternalServerError };
                }

                var originalRoomResult = ResolveOriginalRoom(transferRoomDto);
                if (originalRoomResult.Room == null)
                {
                    return CreateRoomLookupFailure(originalRoomResult, transferRoomDto?.OriginalRoomNumber, transferRoomDto?.OriginalRoomArea, transferRoomDto?.OriginalRoomFloor);
                }

                var targetRoomResult = ResolveTargetRoom(transferRoomDto);
                if (targetRoomResult.Room == null)
                {
                    return CreateRoomLookupFailure(targetRoomResult, transferRoomDto?.TargetRoomNumber, transferRoomDto?.TargetRoomArea, transferRoomDto?.TargetRoomFloor);
                }

                var originalRoom = originalRoomResult.Room;
                var targetRoom = targetRoomResult.Room;
                if (originalRoom.CustomerNumber != transferRoomDto.CustomerNumber)
                {
                    return new BaseResponse { Message = "The customer does not match the original room", Code = BusinessStatusCode.InternalServerError };
                }

                var now = DateTime.Now;
                var originalCheckInTime = NormalizeStayDateTime(originalRoom.LastCheckInTime);
                if (!originalCheckInTime.HasValue)
                {
                    return new BaseResponse { Message = "The original room lacks check-in time", Code = BusinessStatusCode.InternalServerError };
                }

                if (targetRoom.RoomStateId != (int)RoomState.Vacant)
                {
                    return new BaseResponse { Message = "The room is not vacant", Code = BusinessStatusCode.InternalServerError };
                }

                var originalSpends = spendRepository.GetList(a =>
                    a.RoomId.HasValue && a.RoomId.Value == originalRoom.Id
                    && a.CustomerNumber == transferRoomDto.CustomerNumber
                    && a.SettlementStatus == ConsumptionConstant.UnSettle.Code
                    && a.IsDelete == 0);

                var stayDays = CalculateStayDays(originalCheckInTime.Value, now);
                var totalSpent = originalSpends.Sum(a => a.ConsumptionAmount);
                var vipRules = vipLevelRuleRepository.GetList(a => a.IsDelete != 1);
                var newLevelId = vipRules
                    .Where(vipRule => totalSpent >= vipRule.RuleValue)
                    .OrderByDescending(vipRule => vipRule.RuleValue)
                    .ThenByDescending(vipRule => vipRule.VipLevelId)
                    .FirstOrDefault()?.VipLevelId ?? 0;

                if (newLevelId != 0)
                {
                    custoRepository.Update(a => new Domain.Customer { CustomerType = newLevelId }, a => a.CustomerNumber == transferRoomDto.CustomerNumber);
                }

                var customerType = custoTypeRepository.GetFirst(a => a.CustomerType == customer.CustomerType && a.IsDelete != 1);
                if (customerType.IsNullOrEmpty())
                {
                    return new BaseResponse { Message = "The customer type does not exist", Code = BusinessStatusCode.InternalServerError };
                }

                var discount = customerType.Discount > 0 && customerType.Discount < 100 ? customerType.Discount / 100M : 1M;
                var originalPricingEvaluation = EvaluateRoomPricing(originalRoom);
                var originalEffectiveRent = originalPricingEvaluation.EffectiveRoomRent;
                var originalRoomBill = originalEffectiveRent * stayDays * discount;

                targetRoom.CustomerNumber = originalRoom.CustomerNumber;
                targetRoom.RoomStateId = (int)RoomState.Occupied;
                targetRoom.LastCheckInTime = now;
                targetRoom.AppliedRoomRent = originalRoom.AppliedRoomRent;
                targetRoom.AppliedRoomDeposit = originalRoom.AppliedRoomDeposit;
                targetRoom.RoomPricingCode = originalRoom.RoomPricingCode;
                targetRoom.RoomPricingName = originalRoom.RoomPricingName;
                targetRoom.PricingStayHours = originalRoom.PricingStayHours;
                targetRoom.PricingStartTime = NormalizeStayDateTime(originalRoom.PricingStartTime) ?? originalCheckInTime;
                if (!roomRepository.Update(targetRoom))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                originalRoom.CustomerNumber = string.Empty;
                originalRoom.RoomStateId = (int)RoomState.Dirty;
                originalRoom.LastCheckInTime = null;
                originalRoom.LastCheckOutTime = now;
                originalRoom.AppliedRoomRent = 0M;
                originalRoom.AppliedRoomDeposit = 0M;
                originalRoom.RoomPricingCode = string.Empty;
                originalRoom.RoomPricingName = string.Empty;
                originalRoom.PricingStayHours = null;
                originalRoom.PricingStartTime = null;
                if (!roomRepository.Update(originalRoom))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                if (originalSpends.Count > 0)
                {
                    var originalSpendNumbers = originalSpends.Select(a => a.SpendNumber).ToList();
                    var spends = spendRepository.AsQueryable().Where(a => originalSpendNumbers.Contains(a.SpendNumber)).ToList();
                    spends.ForEach(spend =>
                    {
                        spend.RoomId = targetRoom.Id;
                        spend.RoomNumber = targetRoom.RoomNumber;
                    });
                    if (!spendRepository.UpdateRange(spends))
                    {
                        return BaseResponseFactory.ConcurrencyConflict();
                    }
                }

                spendRepository.Insert(new Spend
                {
                    CustomerNumber = transferRoomDto.CustomerNumber,
                    RoomId = targetRoom.Id,
                    RoomNumber = targetRoom.RoomNumber,
                    SpendNumber = uniqueCode.GetNewId("SP-"),
                    ProductNumber = originalRoom.RoomNumber,
                    ProductName = $"居住 {string.Join("/", originalRoom.RoomArea, originalRoom.RoomFloor, originalRoom.RoomNumber)} 共 {stayDays} 天",
                    ProductPrice = originalEffectiveRent,
                    ConsumptionTime = now,
                    SettlementStatus = ConsumptionConstant.UnSettle.Code,
                    ConsumptionQuantity = stayDays,
                    ConsumptionAmount = originalRoomBill,
                    ConsumptionType = SpendTypeConstant.Room.Code,
                    IsDelete = 0
                });

                scope.Complete();
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error transferring room");
                return ErrorResponse(ex);
            }
        }

        public BaseResponse CheckoutRoom(CheckoutRoomDto checkoutRoomDto)
        {
            try
            {
                if (checkoutRoomDto == null || !checkoutRoomDto.RoomId.HasValue || checkoutRoomDto.RoomId.Value <= 0)
                {
                    return new BaseResponse { Message = "RoomId is required", Code = BusinessStatusCode.BadRequest };
                }

                using var scope = CreateTransactionScope();

                var customer = custoRepository.AsQueryable().Where(a => a.CustomerNumber == checkoutRoomDto.CustomerNumber && a.IsDelete != 1);
                if (!customer.Any())
                {
                    return new BaseResponse { Message = "The customer does not exist", Code = BusinessStatusCode.InternalServerError };
                }

                var roomResult = ResolveRoom(checkoutRoomDto);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, checkoutRoomDto?.RoomNumber, checkoutRoomDto?.RoomArea, checkoutRoomDto?.RoomFloor);
                }

                var room = roomResult.Room;
                var now = DateTime.Now;
                var checkinDate = NormalizeStayDateTime(room.LastCheckInTime);
                var occupiedCustomerNumber = room.CustomerNumber;
                var pricingEvaluation = EvaluateRoomPricing(room);
                var effectiveRoomRent = pricingEvaluation.EffectiveRoomRent;

                room.CustomerNumber = string.Empty;
                room.LastCheckInTime = null;
                room.LastCheckOutTime = now;
                room.RoomStateId = (int)RoomState.Dirty;
                room.AppliedRoomRent = 0M;
                room.AppliedRoomDeposit = 0M;
                room.RoomPricingCode = string.Empty;
                room.RoomPricingName = string.Empty;
                room.PricingStayHours = null;
                room.PricingStartTime = null;
                if (!roomRepository.Update(room))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                energyRepository.Insert(new EnergyManagement
                {
                    InformationId = uniqueCode.GetNewId("EM-"),
                    StartDate = checkinDate ?? now,
                    EndDate = now,
                    WaterUsage = checkoutRoomDto.WaterUsage,
                    PowerUsage = checkoutRoomDto.ElectricityUsage,
                    Recorder = "System",
                    CustomerNumber = occupiedCustomerNumber,
                    RoomId = room.Id,
                    RoomNumber = room.RoomNumber,
                    IsDelete = 0
                });

                var unsettledSpends = spendRepository.GetList(a =>
                    a.RoomId.HasValue && a.RoomId.Value == room.Id
                    && a.CustomerNumber == checkoutRoomDto.CustomerNumber
                    && a.SettlementStatus == ConsumptionConstant.UnSettle.Code
                    && a.IsDelete == 0);

                if (unsettledSpends.Count > 0)
                {
                    unsettledSpends.ForEach(spend => spend.SettlementStatus = ConsumptionConstant.Settled.Code);
                    if (!spendRepository.UpdateRange(unsettledSpends))
                    {
                        return BaseResponseFactory.ConcurrencyConflict();
                    }
                }

                var stayDays = CalculateStayDays(checkinDate ?? now, now);
                var customerType = custoTypeRepository.GetSingle(a => a.CustomerType == customer.First().CustomerType && a.IsDelete != 1);
                if (customerType.IsNullOrEmpty())
                {
                    return new BaseResponse { Message = "The customer type does not exist", Code = BusinessStatusCode.InternalServerError };
                }

                var discount = customerType.Discount > 0 && customerType.Discount < 100 ? customerType.Discount / 100M : 1M;
                var roomBill = effectiveRoomRent * stayDays * discount;

                spendRepository.Insert(new Spend
                {
                    SpendNumber = uniqueCode.GetNewId("SP-"),
                    ProductName = $"居住 {string.Join("/", room.RoomArea, room.RoomFloor, room.RoomNumber)} 共 {stayDays} 天",
                    SettlementStatus = ConsumptionConstant.Settled.Code,
                    ConsumptionType = SpendTypeConstant.Room.Code,
                    ConsumptionQuantity = stayDays,
                    ConsumptionTime = now,
                    ProductNumber = room.RoomNumber,
                    ProductPrice = effectiveRoomRent,
                    ConsumptionAmount = roomBill,
                    CustomerNumber = occupiedCustomerNumber,
                    RoomId = room.Id,
                    RoomNumber = room.RoomNumber,
                    IsDelete = 0
                });

                scope.Complete();
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking out room");
                return ErrorResponse(ex);
            }
        }

        public BaseResponse CheckinRoomByReservation(CheckinRoomByReservationDto checkinRoomByReservationDto)
        {
            try
            {
                if (checkinRoomByReservationDto == null
                    || !checkinRoomByReservationDto.RoomId.HasValue
                    || checkinRoomByReservationDto.RoomId.Value <= 0)
                {
                    return new BaseResponse { Message = "RoomId is required", Code = BusinessStatusCode.BadRequest };
                }

                using var scope = CreateTransactionScope();

                var customer = new Domain.Customer
                {
                    CustomerNumber = checkinRoomByReservationDto.CustomerNumber,
                    Name = checkinRoomByReservationDto.CustomerName,
                    Gender = checkinRoomByReservationDto.CustomerGender ?? 0,
                    PhoneNumber = checkinRoomByReservationDto.CustomerPhoneNumber,
                    IdCardType = checkinRoomByReservationDto.PassportId,
                    IdCardNumber = checkinRoomByReservationDto.IdCardNumber,
                    Address = checkinRoomByReservationDto.CustomerAddress,
                    DateOfBirth = checkinRoomByReservationDto.DateOfBirth,
                    CustomerType = checkinRoomByReservationDto.CustomerType,
                    IsDelete = 0,
                    DataInsUsr = checkinRoomByReservationDto.DataInsUsr,
                    DataInsDate = checkinRoomByReservationDto.DataInsDate
                };

                if (!custoRepository.Insert(customer))
                {
                    return new BaseResponse { Message = "Failed to add customer.", Code = BusinessStatusCode.InternalServerError };
                }

                var roomResult = ResolveRoom(checkinRoomByReservationDto);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, checkinRoomByReservationDto?.RoomNumber, checkinRoomByReservationDto?.RoomArea, checkinRoomByReservationDto?.RoomFloor);
                }

                var room = roomResult.Room;
                room.LastCheckInTime = DateTime.Now;
                room.CustomerNumber = customer.CustomerNumber;
                room.RoomStateId = EnumHelper.GetEnumValue(RoomState.Occupied);
                var pricingResponse = ApplyPricingSelection(room, checkinRoomByReservationDto.PricingCode);
                if (pricingResponse != null)
                {
                    return pricingResponse;
                }
                if (!roomRepository.Update(room))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var reser = reserRepository.GetFirst(a => a.ReservationId == checkinRoomByReservationDto.ReservationId && a.IsDelete != 1);
                reser.ReservationStatus = 1;
                reser.IsDelete = 1;
                if (!reserRepository.Update(reser))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                scope.Complete();
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking in room by reservation");
                return ErrorResponse(ex);
            }
        }

        private ListOutputDto<ReadRoomOutputDto> BuildRoomList(ReadRoomInputDto readRoomInputDto)
        {
            readRoomInputDto ??= new ReadRoomInputDto();

            var where = SqlFilterBuilder.BuildExpression<Domain.Room, ReadRoomInputDto>(readRoomInputDto);
            var query = roomRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            query = query.OrderBy(BuildRoomListOrderByClause());

            var count = 0;
            List<Domain.Room> rooms;
            if (!readRoomInputDto.IgnorePaging)
            {
                var page = readRoomInputDto.Page > 0 ? readRoomInputDto.Page : 1;
                var pageSize = readRoomInputDto.PageSize > 0 ? readRoomInputDto.PageSize : 15;
                rooms = query.ToPageList(page, pageSize, ref count);
            }
            else
            {
                rooms = query.ToList();
                count = rooms.Count;
            }

            rooms.ForEach(NormalizeRoom);
            var roomTypeMap = BuildRoomTypeMap();
            var customerMap = BuildCustomerMap(rooms);
            var roomStateMap = BuildRoomStateMap();

            List<ReadRoomOutputDto> result;
            var useParallelProjection = readRoomInputDto.IgnorePaging && rooms.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadRoomOutputDto[rooms.Count];
                System.Threading.Tasks.Parallel.For(0, rooms.Count, i =>
                {
                    dtoArray[i] = MapRoomToOutput(rooms[i], roomTypeMap, customerMap, roomStateMap);
                });
                result = dtoArray.ToList();
            }
            else
            {
                result = rooms.Select(a => MapRoomToOutput(a, roomTypeMap, customerMap, roomStateMap)).ToList();
            }

            return new ListOutputDto<ReadRoomOutputDto>
            {
                Data = new PagedData<ReadRoomOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        private SingleOutputDto<ReadRoomOutputDto> CountByState(RoomState state, Func<int, ReadRoomOutputDto> builder)
        {
            try
            {
                var count = roomRepository.Count(a => a.RoomStateId == (int)state && a.IsDelete != 1);
                return new SingleOutputDto<ReadRoomOutputDto> { Data = builder(count) };
            }
            catch (Exception ex)
            {
                return new SingleOutputDto<ReadRoomOutputDto> { Code = BusinessStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        private RoomResolveResult ResolveRoom(ReadRoomInputDto inputDto) => RoomLocatorHelper.Resolve(roomRepository, inputDto?.Id, inputDto?.RoomNumber, inputDto?.RoomArea, inputDto?.RoomFloor);

        private RoomResolveResult ResolveRoom(UpdateRoomInputDto inputDto) => RoomLocatorHelper.Resolve(roomRepository, inputDto?.Id, inputDto?.RoomNumber, inputDto?.RoomArea, inputDto?.RoomFloor);

        private RoomResolveResult ResolveRoom(CheckoutRoomDto inputDto) => RoomLocatorHelper.Resolve(roomRepository, inputDto?.RoomId, null, null, null);

        private RoomResolveResult ResolveRoom(CheckinRoomByReservationDto inputDto) => RoomLocatorHelper.Resolve(roomRepository, inputDto?.RoomId, null, null, null);

        private RoomResolveResult ResolveOriginalRoom(TransferRoomDto inputDto) => RoomLocatorHelper.Resolve(roomRepository, inputDto?.OriginalRoomId, null, null, null);

        private RoomResolveResult ResolveTargetRoom(TransferRoomDto inputDto) => RoomLocatorHelper.Resolve(roomRepository, inputDto?.TargetRoomId, null, null, null);

        private static void NormalizeRoom(Domain.Room room)
        {
            if (room == null)
            {
                return;
            }

            room.RoomNumber = room.RoomNumber?.Trim();
            room.RoomArea = string.IsNullOrWhiteSpace(room.RoomArea) ? null : RoomLocatorHelper.NormalizeArea(room.RoomArea);
            room.RoomLocator = RoomLocatorHelper.BuildLocator(room.RoomArea, room.RoomFloor, room.RoomNumber);
        }

        private static BaseResponse CreateRoomLookupFailure(RoomResolveResult result, string roomNumber, string roomArea, int? roomFloor)
        {
            var locator = RoomLocatorHelper.BuildLocator(roomArea, roomFloor, roomNumber);
            if (result?.IsAmbiguous == true)
            {
                return new BaseResponse { Code = BusinessStatusCode.Conflict, Message = $"Multiple rooms match '{locator}'. Please specify room area or floor." };
            }

            if (string.IsNullOrWhiteSpace(locator))
            {
                return new BaseResponse { Code = BusinessStatusCode.NotFound, Message = "RoomId was not found." };
            }

            return new BaseResponse { Code = BusinessStatusCode.NotFound, Message = $"Room '{locator}' was not found." };
        }

        private static SingleOutputDto<ReadRoomOutputDto> CreateRoomLookupFailureOutput(RoomResolveResult result, string roomNumber, string roomArea, int? roomFloor)
        {
            var response = CreateRoomLookupFailure(result, roomNumber, roomArea, roomFloor);
            return new SingleOutputDto<ReadRoomOutputDto> { Code = response.Code, Message = response.Message, Data = new ReadRoomOutputDto() };
        }

        private string BuildRoomListOrderByClause()
        {
            var entityInfo = roomRepository.Context.EntityMaintenance.GetEntityInfo<Domain.Room>();
            var orderedColumns = new[]
            {
                nameof(Domain.Room.RoomArea),
                nameof(Domain.Room.RoomFloor),
                nameof(Domain.Room.RoomNumber)
            };

            return string.Join(", ", orderedColumns.Select(propertyName =>
            {
                var columnInfo = entityInfo.Columns.FirstOrDefault(a => a.PropertyName == propertyName);
                var columnName = columnInfo?.DbColumnName ?? propertyName;
                return $"{columnName} asc";
            }));
        }

        private Dictionary<int, string> BuildRoomTypeMap()
        {
            return roomTypeRepository.AsQueryable()
                .Where(a => a.IsDelete != 1)
                .ToList()
                .GroupBy(a => a.RoomTypeId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.RoomTypeName ?? "");
        }

        private Dictionary<string, string> BuildCustomerMap(List<Domain.Room> rooms)
        {
            var customerNumbers = rooms
                .Select(a => a.CustomerNumber)
                .Where(a => !a.IsNullOrEmpty())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (customerNumbers.Count == 0)
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            return custoRepository.AsQueryable()
                .Where(a => a.IsDelete != 1 && customerNumbers.Contains(a.CustomerNumber))
                .ToList()
                .GroupBy(a => a.CustomerNumber)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.Name ?? "", StringComparer.OrdinalIgnoreCase);
        }

        private static Dictionary<int, string> BuildRoomStateMap()
        {
            return Enum.GetValues(typeof(RoomState)).Cast<RoomState>().ToDictionary(e => (int)e, e => EnumHelper.GetEnumDescription(e) ?? "");
        }

        private static ReadRoomOutputDto MapRoomToOutput(Domain.Room source, Dictionary<int, string> roomTypeMap, Dictionary<string, string> customerMap, Dictionary<int, string> roomStateMap)
        {
            var pricingEvaluation = EvaluateRoomPricing(source);
            return new ReadRoomOutputDto
            {
                Id = source.Id,
                RoomNumber = source.RoomNumber,
                RoomArea = source.RoomArea ?? string.Empty,
                RoomFloor = source.RoomFloor,
                RoomLocator = RoomLocatorHelper.BuildLocator(source.RoomArea, source.RoomFloor, source.RoomNumber),
                RoomTypeId = source.RoomTypeId,
                RoomName = roomTypeMap.TryGetValue(source.RoomTypeId, out var roomTypeName) ? roomTypeName : "",
                CustomerNumber = source.CustomerNumber ?? "",
                CustomerName = customerMap.TryGetValue(source.CustomerNumber ?? "", out var customerName) ? customerName : "",
                LastCheckInTime = NormalizeStayDateTime(source.LastCheckInTime),
                LastCheckOutTime = NormalizeStayDateTime(source.LastCheckOutTime),
                RoomStateId = source.RoomStateId,
                RoomState = roomStateMap.TryGetValue(source.RoomStateId, out var roomStateName) ? roomStateName : "",
                RoomRent = pricingEvaluation.EffectiveRoomRent,
                RoomDeposit = pricingEvaluation.EffectiveRoomDeposit,
                StandardRoomRent = source.RoomRent,
                StandardRoomDeposit = source.RoomDeposit,
                AppliedRoomRent = source.AppliedRoomRent,
                AppliedRoomDeposit = source.AppliedRoomDeposit,
                EffectiveRoomRent = pricingEvaluation.EffectiveRoomRent,
                EffectiveRoomDeposit = pricingEvaluation.EffectiveRoomDeposit,
                EffectivePricingCode = pricingEvaluation.EffectivePricingCode,
                EffectivePricingName = pricingEvaluation.EffectivePricingName,
                PricingCode = pricingEvaluation.SelectedPricingCode,
                PricingName = pricingEvaluation.SelectedPricingName,
                PricingStayHours = pricingEvaluation.PricingStayHours,
                IsPricingTimedOut = pricingEvaluation.IsPricingTimedOut,
                RoomLocation = source.RoomLocation,
                DataInsUsr = source.DataInsUsr,
                DataInsDate = source.DataInsDate,
                DataChgUsr = source.DataChgUsr,
                DataChgDate = source.DataChgDate,
                RowVersion = source.RowVersion,
                IsDelete = source.IsDelete
            };
        }

        private static BaseResponse ErrorResponse(Exception ex)
        {
            return new BaseResponse { Message = ex.Message, Code = BusinessStatusCode.InternalServerError };
        }

        private BaseResponse ApplyPricingSelection(Domain.Room room, string pricingCode)
        {
            if (room == null || string.IsNullOrWhiteSpace(pricingCode))
            {
                return null;
            }

            var roomType = roomTypeRepository.GetFirst(a => a.RoomTypeId == room.RoomTypeId && a.IsDelete != 1);
            if (roomType == null)
            {
                return new BaseResponse { Message = "Room type pricing not found", Code = BusinessStatusCode.NotFound };
            }

            var pricingItem = RoomPricingHelper.ResolvePricingItem(roomType.RoomRent, roomType.RoomDeposit, roomType.PricingItemsJson, pricingCode);
            if (pricingItem == null)
            {
                return new BaseResponse { Message = "Pricing code not found", Code = BusinessStatusCode.BadRequest };
            }

            if (pricingItem.IsDefault)
            {
                room.AppliedRoomRent = 0M;
                room.AppliedRoomDeposit = 0M;
                room.RoomPricingCode = string.Empty;
                room.RoomPricingName = string.Empty;
                room.PricingStayHours = null;
                room.PricingStartTime = null;
                return null;
            }

            room.AppliedRoomRent = pricingItem.RoomRent;
            room.AppliedRoomDeposit = pricingItem.RoomDeposit;
            room.RoomPricingCode = pricingItem.PricingCode;
            room.RoomPricingName = pricingItem.PricingName;
            room.PricingStayHours = pricingItem.StayHours > 0 ? pricingItem.StayHours : null;
            room.PricingStartTime = room.PricingStayHours.HasValue
                ? NormalizeStayDateTime(room.LastCheckInTime) ?? DateTime.Now
                : null;
            return null;
        }

        private static decimal GetEffectiveRoomRent(Domain.Room room)
        {
            return EvaluateRoomPricing(room).EffectiveRoomRent;
        }

        private static decimal GetEffectiveRoomDeposit(Domain.Room room)
        {
            return EvaluateRoomPricing(room).EffectiveRoomDeposit;
        }

        private static RoomPricingEvaluation EvaluateRoomPricing(Domain.Room room)
        {
            if (room == null)
            {
                return new RoomPricingEvaluation
                {
                    SelectedPricingCode = RoomPricingHelper.DefaultPricingCode,
                    SelectedPricingName = RoomPricingHelper.DefaultPricingName,
                    EffectivePricingCode = RoomPricingHelper.DefaultPricingCode,
                    EffectivePricingName = RoomPricingHelper.DefaultPricingName,
                    EffectiveRoomRent = 0M,
                    EffectiveRoomDeposit = 0M
                };
            }

            var selectedPricingCode = string.IsNullOrWhiteSpace(room.RoomPricingCode) ? RoomPricingHelper.DefaultPricingCode : room.RoomPricingCode;
            var selectedPricingName = string.IsNullOrWhiteSpace(room.RoomPricingName) ? RoomPricingHelper.DefaultPricingName : room.RoomPricingName;
            var effectivePricingCode = RoomPricingHelper.DefaultPricingCode;
            var effectivePricingName = RoomPricingHelper.DefaultPricingName;
            var effectiveRoomRent = room.RoomRent;
            var effectiveRoomDeposit = room.RoomDeposit;
            var isPricingTimedOut = false;

            if (room.AppliedRoomRent > 0 || room.AppliedRoomDeposit > 0 || !string.IsNullOrWhiteSpace(room.RoomPricingCode))
            {
                isPricingTimedOut = IsPricingTimedOut(room);
                if (!isPricingTimedOut)
                {
                    effectivePricingCode = selectedPricingCode;
                    effectivePricingName = selectedPricingName;
                    effectiveRoomRent = room.AppliedRoomRent > 0 ? room.AppliedRoomRent : room.RoomRent;
                    effectiveRoomDeposit = room.AppliedRoomDeposit > 0 ? room.AppliedRoomDeposit : room.RoomDeposit;
                }
            }

            return new RoomPricingEvaluation
            {
                SelectedPricingCode = selectedPricingCode,
                SelectedPricingName = selectedPricingName,
                EffectivePricingCode = effectivePricingCode,
                EffectivePricingName = effectivePricingName,
                EffectiveRoomRent = effectiveRoomRent,
                EffectiveRoomDeposit = effectiveRoomDeposit,
                PricingStayHours = room.PricingStayHours > 0 ? room.PricingStayHours : null,
                IsPricingTimedOut = isPricingTimedOut
            };
        }

        private static bool IsPricingTimedOut(Domain.Room room)
        {
            if (room?.PricingStayHours is not > 0)
            {
                return false;
            }

            var pricingStartTime = NormalizeStayDateTime(room.PricingStartTime)
                ?? NormalizeStayDateTime(room.LastCheckInTime);
            if (!pricingStartTime.HasValue)
            {
                return false;
            }

            var now = DateTime.Now;
            return now > pricingStartTime.Value.AddHours(room.PricingStayHours.Value);
        }

        private static TransactionScope CreateTransactionScope()
        {
            return new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromSeconds(30)
                });
        }

        private static int CalculateStayDays(DateTime checkInTime, DateTime referenceTime)
        {
            var staySpan = referenceTime - checkInTime;
            return Math.Max((int)Math.Ceiling(staySpan.TotalDays), 1);
        }

        private static DateTime? NormalizeStayDateTime(DateTime? value)
        {
            return value.HasValue && value.Value > DateTime.MinValue ? value : null;
        }
    }
}
