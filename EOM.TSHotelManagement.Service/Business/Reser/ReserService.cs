using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Common.Helper;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace EOM.TSHotelManagement.Service.Business.Reser
{
    public class ReserService(
        GenericRepository<Domain.Reser> reserRepository,
        GenericRepository<Domain.Room> roomRepository,
        DataProtectionHelper dataProtector,
        ILogger<ReserService> logger) : IReserService
    {

        public ListOutputDto<ReadReserOutputDto> SelectReserAll(ReadReserInputDto readReserInputDto)
        {
            readReserInputDto ??= new ReadReserInputDto();
            var filterInput = CreateReservationFilter(readReserInputDto);

            var reserTypeMap = Enum.GetValues(typeof(ReserType))
                .Cast<ReserType>()
                .ToDictionary(a => a.ToString(), a => EnumHelper.GetEnumDescription(a) ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            var where = SqlFilterBuilder.BuildExpression<Domain.Reser, ReadReserInputDto>(filterInput, new Dictionary<string, string>
            {
                { nameof(ReadReserInputDto.ReservationStartDateStart), nameof(Domain.Reser.ReservationStartDate) },
                { nameof(ReadReserInputDto.ReservationStartDateEnd), nameof(Domain.Reser.ReservationEndDate) }
            });

            var query = reserRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            if (readReserInputDto.RoomId.HasValue && readReserInputDto.RoomId.Value > 0)
            {
                query = query.Where(a => a.RoomId == readReserInputDto.RoomId.Value);
            }

            var count = 0;
            List<Domain.Reser> reservations;
            if (readReserInputDto.IgnorePaging)
            {
                reservations = query.ToList();
                count = reservations.Count;
            }
            else
            {
                var page = readReserInputDto.Page > 0 ? readReserInputDto.Page : 1;
                var pageSize = readReserInputDto.PageSize > 0 ? readReserInputDto.PageSize : 15;
                reservations = query.ToPageList(page, pageSize, ref count);
            }

            var rooms = RoomReferenceHelper.LoadRooms(roomRepository, reservations.Select(a => a.RoomId), reservations.Select(a => a.ReservationRoomNumber));
            var outputs = reservations
                .Select(a => MapReservationToOutput(a, RoomReferenceHelper.FindRoom(rooms, a.RoomId, a.ReservationRoomNumber), reserTypeMap))
                .ToList();

            return new ListOutputDto<ReadReserOutputDto>
            {
                Data = new PagedData<ReadReserOutputDto>
                {
                    Items = outputs,
                    TotalCount = count
                }
            };
        }

        public SingleOutputDto<ReadReserOutputDto> SelectReserInfoByRoomNo(ReadReserInputDto readReserInputDt)
        {
            var reserTypeMap = Enum.GetValues(typeof(ReserType))
                .Cast<ReserType>()
                .ToDictionary(a => a.ToString(), a => EnumHelper.GetEnumDescription(a) ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            var query = reserRepository.AsQueryable().Where(a => a.ReservationStatus == 0 && a.IsDelete != 1);
            if (readReserInputDt?.RoomId > 0)
            {
                query = query.Where(a => a.RoomId == readReserInputDt.RoomId);
            }
            else
            {
                return new SingleOutputDto<ReadReserOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = "RoomId is required.",
                    Data = new ReadReserOutputDto()
                };
            }

            var reservation = query.First();
            if (reservation == null)
            {
                return new SingleOutputDto<ReadReserOutputDto>
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = "Reservation not found.",
                    Data = new ReadReserOutputDto()
                };
            }

            var room = RoomReferenceHelper.Resolve(roomRepository, reservation.RoomId, reservation.ReservationRoomNumber).Room;
            return new SingleOutputDto<ReadReserOutputDto>
            {
                Data = MapReservationToOutput(reservation, room, reserTypeMap)
            };
        }

        public BaseResponse DeleteReserInfo(DeleteReserInputDto reser)
        {
            if (reser?.DelIds == null || !reser.DelIds.Any())
            {
                return new BaseResponse(BusinessStatusCode.BadRequest, "Parameters invalid.");
            }

            var delIds = DeleteConcurrencyHelper.GetDeleteIds(reser);
            var reservations = reserRepository.GetList(a => delIds.Contains(a.Id));
            if (!reservations.Any())
            {
                return new BaseResponse(BusinessStatusCode.NotFound, "Reservation information not found.");
            }

            if (DeleteConcurrencyHelper.HasDeleteConflict(reser, reservations, a => a.Id, a => a.RowVersion))
            {
                return BaseResponseFactory.ConcurrencyConflict();
            }

            try
            {
                using var scope = new TransactionScope();
                if (!reserRepository.SoftDeleteRange(reservations))
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError, "Delete reservation failed.");
                }

                var rooms = RoomReferenceHelper.LoadRooms(roomRepository, reservations.Select(a => a.RoomId), reservations.Select(a => a.ReservationRoomNumber));
                if (rooms.Count > 0)
                {
                    var remainingReservations = reserRepository.AsQueryable()
                        .Where(a => a.IsDelete != 1 && a.ReservationStatus == 0)
                        .ToList();

                    var changedRooms = new List<Domain.Room>();
                    foreach (var room in rooms)
                    {
                        var hasOtherActiveReservation = remainingReservations.Any(a => IsSameRoom(a, room));
                        if (!hasOtherActiveReservation)
                        {
                            room.RoomStateId = (int)RoomState.Vacant;
                            changedRooms.Add(room);
                        }
                    }

                    if (changedRooms.Count > 0 && !roomRepository.UpdateRange(changedRooms))
                    {
                        return BaseResponseFactory.ConcurrencyConflict();
                    }
                }

                scope.Complete();
                return new BaseResponse(BusinessStatusCode.Success, "Delete reservation success.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Delete reservation failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, "Delete reservation failed.");
            }
        }

        public BaseResponse UpdateReserInfo(UpdateReserInputDto reser)
        {
            try
            {
                using var scope = new TransactionScope();

                var originalReser = reserRepository.GetFirst(a => a.Id == reser.Id);
                if (originalReser == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, "Reservation not found.");
                }

                var targetRoomResult = RoomReferenceHelper.Resolve(roomRepository, reser.RoomId ?? originalReser.RoomId, reser.ReservationRoomNumber ?? originalReser.ReservationRoomNumber);
                if (targetRoomResult.Room == null)
                {
                    return CreateRoomLookupFailure(targetRoomResult, reser.RoomId ?? originalReser.RoomId, reser.ReservationRoomNumber ?? originalReser.ReservationRoomNumber);
                }

                var targetRoom = targetRoomResult.Room;
                var startDate = DateOnly.FromDateTime(reser.ReservationStartDate);
                var endDate = DateOnly.FromDateTime(reser.ReservationEndDate);

                var isRestoring = originalReser.IsDelete == 1 && reser.IsDelete == 0;
                if (isRestoring && targetRoom.RoomStateId != (int)RoomState.Vacant)
                {
                    return new BaseResponse(BusinessStatusCode.Conflict, "Room is not vacant.");
                }

                var hasConflict = reserRepository.AsQueryable().Any(a =>
                    a.Id != originalReser.Id &&
                    a.IsDelete != 1 &&
                    a.ReservationStatus == 0 &&
                    a.RoomId == targetRoom.Id &&
                    a.ReservationStartDate < endDate &&
                    a.ReservationEndDate > startDate);

                if (hasConflict)
                {
                    return new BaseResponse(BusinessStatusCode.Conflict, "Room is already reserved during this period.");
                }

                originalReser.ReservationId = reser.ReservationId;
                originalReser.CustomerName = reser.CustomerName;
                originalReser.ReservationPhoneNumber = dataProtector.EncryptReserData(reser.ReservationPhoneNumber);
                originalReser.RoomId = targetRoom.Id;
                originalReser.ReservationRoomNumber = targetRoom.RoomNumber;
                originalReser.ReservationChannel = reser.ReservationChannel;
                originalReser.ReservationStartDate = startDate;
                originalReser.ReservationEndDate = endDate;
                originalReser.IsDelete = reser.IsDelete;
                originalReser.DataChgUsr = reser.DataChgUsr;
                originalReser.DataChgDate = reser.DataChgDate;
                originalReser.RowVersion = reser.RowVersion ?? 0;

                if (!reserRepository.Update(originalReser))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                if (originalReser.IsDelete != 1 && originalReser.ReservationStatus == 0)
                {
                    targetRoom.RoomStateId = (int)RoomState.Reserved;
                    if (!roomRepository.Update(targetRoom))
                    {
                        return BaseResponseFactory.ConcurrencyConflict();
                    }
                }

                scope.Complete();
                return new BaseResponse(BusinessStatusCode.Success, "Update reservation success.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update reservation failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, "Update reservation failed.");
            }
        }

        public BaseResponse InserReserInfo(CreateReserInputDto input)
        {
            try
            {
                var roomResult = RoomReferenceHelper.Resolve(roomRepository, input?.RoomId, input?.ReservationRoomNumber);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, input?.RoomId, input?.ReservationRoomNumber);
                }

                var entity = new Domain.Reser
                {
                    ReservationId = input.ReservationId,
                    CustomerName = input.CustomerName,
                    ReservationPhoneNumber = dataProtector.EncryptReserData(input.ReservationPhoneNumber),
                    RoomId = roomResult.Room.Id,
                    ReservationRoomNumber = roomResult.Room.RoomNumber,
                    ReservationChannel = input.ReservationChannel,
                    ReservationStartDate = DateOnly.FromDateTime(input.ReservationStartDate),
                    ReservationEndDate = DateOnly.FromDateTime(input.ReservationEndDate),
                    ReservationStatus = input.ReservationStatus,
                    DataInsUsr = input.DataInsUsr,
                    DataInsDate = input.DataInsDate
                };

                reserRepository.Insert(entity);

                roomResult.Room.RoomStateId = EnumHelper.GetEnumValue(RoomState.Reserved);
                if (!roomRepository.Update(roomResult.Room))
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError, "Insert reservation failed.");
                }

                return new BaseResponse(BusinessStatusCode.Success, "Insert reservation success.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Insert reservation failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, "Insert reservation failed.");
            }
        }

        public ListOutputDto<EnumDto> SelectReserTypeAll()
        {
            var enumList = Enum.GetValues(typeof(ReserType))
                .Cast<ReserType>()
                .Select(a => new EnumDto
                {
                    Id = (int)a,
                    Name = a.ToString(),
                    Description = EnumHelper.GetEnumDescription(a)
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

        private ReadReserOutputDto MapReservationToOutput(Domain.Reser source, Domain.Room room, Dictionary<string, string> reserTypeMap)
        {
            return new ReadReserOutputDto
            {
                Id = source.Id,
                RoomId = source.RoomId ?? room?.Id,
                ReservationId = source.ReservationId,
                CustomerName = source.CustomerName,
                ReservationPhoneNumber = dataProtector.SafeDecryptReserData(source.ReservationPhoneNumber),
                ReservationRoomNumber = room?.RoomNumber ?? source.ReservationRoomNumber,
                RoomArea = RoomReferenceHelper.GetRoomArea(room),
                RoomFloor = RoomReferenceHelper.GetRoomFloor(room),
                RoomLocator = RoomReferenceHelper.GetRoomLocator(room),
                ReservationChannel = source.ReservationChannel,
                ReservationChannelDescription = reserTypeMap.TryGetValue(source.ReservationChannel ?? string.Empty, out var channelDescription) ? channelDescription : string.Empty,
                ReservationStartDate = source.ReservationStartDate.ToDateTime(TimeOnly.MinValue),
                ReservationEndDate = source.ReservationEndDate.ToDateTime(TimeOnly.MinValue),
                DataInsUsr = source.DataInsUsr,
                DataInsDate = source.DataInsDate,
                DataChgUsr = source.DataChgUsr,
                DataChgDate = source.DataChgDate,
                RowVersion = source.RowVersion,
                IsDelete = source.IsDelete
            };
        }

        private static bool IsSameRoom(Domain.Reser reservation, Domain.Room room)
        {
            if (reservation == null || room == null || !reservation.RoomId.HasValue || reservation.RoomId.Value <= 0)
            {
                return false;
            }

            return reservation.RoomId.Value == room.Id;
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

        private static ReadReserInputDto CreateReservationFilter(ReadReserInputDto input)
        {
            return new ReadReserInputDto
            {
                Page = input.Page,
                PageSize = input.PageSize,
                IgnorePaging = input.IgnorePaging,
                RoomId = null,
                ReservationId = input.ReservationId,
                CustomerName = input.CustomerName,
                ReservationPhoneNumber = input.ReservationPhoneNumber,
                ReservationRoomNumber = null,
                ReservationChannel = input.ReservationChannel,
                ReservationStartDateStart = input.ReservationStartDateStart,
                ReservationStartDateEnd = input.ReservationStartDateEnd
            };
        }
    }
}
