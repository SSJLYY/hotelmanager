using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EOM.TSHotelManagement.Service
{
    public class EnergyManagementService : IEnergyManagementService
    {
        private readonly GenericRepository<EnergyManagement> wtiRepository;
        private readonly GenericRepository<Room> roomRepository;
        private readonly ILogger<EnergyManagementService> logger;

        public EnergyManagementService(
            GenericRepository<EnergyManagement> wtiRepository,
            GenericRepository<Room> roomRepository,
            ILogger<EnergyManagementService> logger)
        {
            this.wtiRepository = wtiRepository;
            this.roomRepository = roomRepository;
            this.logger = logger;
        }

        public ListOutputDto<ReadEnergyManagementOutputDto> SelectEnergyManagementInfo(ReadEnergyManagementInputDto readEnergyManagementInputDto)
        {
            readEnergyManagementInputDto ??= new ReadEnergyManagementInputDto();
            var filterInput = CreateEnergyFilter(readEnergyManagementInputDto);

            var where = SqlFilterBuilder.BuildExpression<EnergyManagement, ReadEnergyManagementInputDto>(filterInput);
            var query = wtiRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            if (readEnergyManagementInputDto.RoomId.HasValue && readEnergyManagementInputDto.RoomId.Value > 0)
            {
                query = query.Where(a => a.RoomId == readEnergyManagementInputDto.RoomId.Value);
            }

            var count = 0;
            List<EnergyManagement> data;
            if (readEnergyManagementInputDto.IgnorePaging)
            {
                data = query.ToList();
                count = data.Count;
            }
            else
            {
                var page = readEnergyManagementInputDto.Page > 0 ? readEnergyManagementInputDto.Page : 1;
                var pageSize = readEnergyManagementInputDto.PageSize > 0 ? readEnergyManagementInputDto.PageSize : 15;
                data = query.ToPageList(page, pageSize, ref count);
            }

            var rooms = RoomReferenceHelper.LoadRooms(roomRepository, data.Select(a => a.RoomId), data.Select(a => a.RoomNumber));
            var outputs = data.Select(a => MapEnergyToOutput(a, RoomReferenceHelper.FindRoom(rooms, a.RoomId, a.RoomNumber))).ToList();

            return new ListOutputDto<ReadEnergyManagementOutputDto>
            {
                Data = new PagedData<ReadEnergyManagementOutputDto>
                {
                    Items = outputs,
                    TotalCount = count
                }
            };
        }

        public BaseResponse InsertEnergyManagementInfo(CreateEnergyManagementInputDto input)
        {
            try
            {
                if (wtiRepository.IsAny(a => a.InformationId == input.InformationNumber && a.IsDelete != 1))
                {
                    return new BaseResponse(BusinessStatusCode.Conflict, "Information number already exists.");
                }

                var roomResult = RoomReferenceHelper.Resolve(roomRepository, input?.RoomId, input?.RoomNumber);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, input?.RoomId, input?.RoomNumber);
                }

                var entity = EntityMapper.Map<CreateEnergyManagementInputDto, EnergyManagement>(input);
                entity.InformationId = input.InformationNumber;
                entity.RoomId = roomResult.Room.Id;
                entity.RoomNumber = roomResult.Room.RoomNumber;
                entity.StartDate = input.StartDate;
                entity.EndDate = input.EndDate;

                wtiRepository.Insert(entity);
                return new BaseResponse(BusinessStatusCode.Success, "Insert energy management success.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Insert energy management failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, "Insert energy management failed.");
            }
        }

        public BaseResponse UpdateEnergyManagementInfo(UpdateEnergyManagementInputDto input)
        {
            try
            {
                var entity = wtiRepository.GetFirst(a => a.InformationId == input.InformationId && a.IsDelete != 1);
                if (entity == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, "Information number does not exist.");
                }

                var roomResult = RoomReferenceHelper.Resolve(roomRepository, input?.RoomId, input?.RoomNumber);
                if (roomResult.Room == null)
                {
                    return CreateRoomLookupFailure(roomResult, input?.RoomId, input?.RoomNumber);
                }

                entity.RoomId = roomResult.Room.Id;
                entity.RoomNumber = roomResult.Room.RoomNumber;
                entity.CustomerNumber = input.CustomerNumber;
                entity.StartDate = input.StartDate;
                entity.EndDate = input.EndDate;
                entity.PowerUsage = input.PowerUsage;
                entity.WaterUsage = input.WaterUsage;
                entity.Recorder = input.Recorder;
                entity.DataChgUsr = input.DataChgUsr;
                entity.DataChgDate = input.DataChgDate;
                entity.RowVersion = input.RowVersion ?? 0;

                return wtiRepository.Update(entity)
                    ? new BaseResponse(BusinessStatusCode.Success, "Update energy management success.")
                    : BaseResponseFactory.ConcurrencyConflict();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update energy management failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, "Update energy management failed.");
            }
        }

        public BaseResponse DeleteEnergyManagementInfo(DeleteEnergyManagementInputDto hydroelectricity)
        {
            try
            {
                if (hydroelectricity?.DelIds == null || !hydroelectricity.DelIds.Any())
                {
                    return new BaseResponse(BusinessStatusCode.BadRequest, "Parameters invalid.");
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(hydroelectricity);
                var energyManagements = wtiRepository.GetList(a => delIds.Contains(a.Id));
                if (!energyManagements.Any())
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, "Energy management information not found.");
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(hydroelectricity, energyManagements, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                return wtiRepository.SoftDeleteRange(energyManagements)
                    ? new BaseResponse(BusinessStatusCode.Success, "Delete energy management success.")
                    : new BaseResponse(BusinessStatusCode.InternalServerError, "Delete energy management failed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Delete energy management failed");
                return new BaseResponse(BusinessStatusCode.InternalServerError, "Delete energy management failed.");
            }
        }

        private static ReadEnergyManagementOutputDto MapEnergyToOutput(EnergyManagement source, Room room)
        {
            return new ReadEnergyManagementOutputDto
            {
                Id = source.Id,
                InformationId = source.InformationId,
                RoomId = source.RoomId ?? room?.Id,
                RoomNumber = room?.RoomNumber ?? source.RoomNumber,
                RoomArea = RoomReferenceHelper.GetRoomArea(room),
                RoomFloor = RoomReferenceHelper.GetRoomFloor(room),
                RoomLocator = RoomReferenceHelper.GetRoomLocator(room),
                CustomerNumber = source.CustomerNumber,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                PowerUsage = source.PowerUsage,
                WaterUsage = source.WaterUsage,
                Recorder = source.Recorder,
                DataInsUsr = source.DataInsUsr,
                DataInsDate = source.DataInsDate,
                DataChgUsr = source.DataChgUsr,
                DataChgDate = source.DataChgDate,
                RowVersion = source.RowVersion,
                IsDelete = source.IsDelete
            };
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

        private static ReadEnergyManagementInputDto CreateEnergyFilter(ReadEnergyManagementInputDto input)
        {
            return new ReadEnergyManagementInputDto
            {
                Page = input.Page,
                PageSize = input.PageSize,
                IgnorePaging = input.IgnorePaging,
                CustomerNumber = input.CustomerNumber,
                RoomId = null,
                RoomNumber = null,
                StartDate = input.StartDate,
                EndDate = input.EndDate
            };
        }
    }
}
