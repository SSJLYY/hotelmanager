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
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using Microsoft.Extensions.Logging;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 房间类型接口实现类
    /// </summary>
    public class RoomTypeService : IRoomTypeService
    {
        /// <summary>
        /// 客房类型
        /// </summary>
        private readonly GenericRepository<RoomType> roomTypeRepository;

        /// <summary>
        /// 客房信息
        /// </summary>
        private readonly GenericRepository<Room> roomRepository;
        private readonly ILogger<RoomTypeService> logger;

        public RoomTypeService(GenericRepository<RoomType> roomTypeRepository, GenericRepository<Room> roomRepository, ILogger<RoomTypeService> logger)
        {
            this.roomTypeRepository = roomTypeRepository;
            this.roomRepository = roomRepository;
            this.logger = logger;
        }

        #region 获取所有房间类型
        /// <summary>
        /// 获取所有房间类型
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadRoomTypeOutputDto> SelectRoomTypesAll(ReadRoomTypeInputDto readRoomTypeInputDto)
        {
            readRoomTypeInputDto ??= new ReadRoomTypeInputDto();

            var where = SqlFilterBuilder.BuildExpression<RoomType, ReadRoomTypeInputDto>(readRoomTypeInputDto);
            var query = roomTypeRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            var count = 0;
            List<RoomType> types;

            if (!readRoomTypeInputDto.IgnorePaging)
            {
                var page = readRoomTypeInputDto.Page > 0 ? readRoomTypeInputDto.Page : 1;
                var pageSize = readRoomTypeInputDto.PageSize > 0 ? readRoomTypeInputDto.PageSize : 15;
                types = query.ToPageList(page, pageSize, ref count);
            }
            else
            {
                types = query.ToList();
                count = types.Count;
            }

            List<ReadRoomTypeOutputDto> mapped;
            var useParallelProjection = readRoomTypeInputDto.IgnorePaging && types.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadRoomTypeOutputDto[types.Count];
                System.Threading.Tasks.Parallel.For(0, types.Count, i =>
                {
                    var source = types[i];
                    dtoArray[i] = new ReadRoomTypeOutputDto
                    {
                        Id = source.Id,
                        RoomTypeId = source.RoomTypeId,
                        RoomTypeName = source.RoomTypeName,
                        RoomRent = source.RoomRent,
                        RoomDeposit = source.RoomDeposit,
                        PricingItems = RoomPricingHelper.BuildPricingItems(source.RoomRent, source.RoomDeposit, source.PricingItemsJson),
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete,
                    };
                });
                mapped = dtoArray.ToList();
            }
            else
            {
                mapped = new List<ReadRoomTypeOutputDto>(types.Count);
                types.ForEach(source =>
                {
                    mapped.Add(new ReadRoomTypeOutputDto
                    {
                        Id = source.Id,
                        RoomTypeId = source.RoomTypeId,
                        RoomTypeName = source.RoomTypeName,
                        RoomRent = source.RoomRent,
                        RoomDeposit = source.RoomDeposit,
                        PricingItems = RoomPricingHelper.BuildPricingItems(source.RoomRent, source.RoomDeposit, source.PricingItemsJson),
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    });
                });
            }

            return new ListOutputDto<ReadRoomTypeOutputDto>
            {
                Data = new PagedData<ReadRoomTypeOutputDto>
                {
                    Items = mapped,
                    TotalCount = count
                }
            };
        }
        #endregion

        #region 根据房间编号查询房间类型名称
        /// <summary>
        /// 根据房间编号查询房间类型名称
        /// </summary>
        /// <param name="readRoomTypeInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadRoomTypeOutputDto> SelectRoomTypeByRoomNo(ReadRoomTypeInputDto readRoomTypeInputDto)
        {
            var roomResult = RoomLocatorHelper.Resolve(roomRepository, readRoomTypeInputDto?.Id, readRoomTypeInputDto?.RoomNumber, readRoomTypeInputDto?.RoomArea, readRoomTypeInputDto?.RoomFloor);
            if (roomResult.Room == null)
            {
                return new SingleOutputDto<ReadRoomTypeOutputDto>
                {
                    Code = roomResult.IsAmbiguous ? BusinessStatusCode.Conflict : BusinessStatusCode.NotFound,
                    Message = roomResult.IsAmbiguous ? "Multiple rooms match the current room number." : "Room not found.",
                    Data = new ReadRoomTypeOutputDto()
                };
            }

            var roomType = roomTypeRepository.GetFirst(a => a.RoomTypeId == roomResult.Room.RoomTypeId && a.IsDelete != 1) ?? new RoomType();
            var source = MapRoomTypeToOutput(roomType);
            return new SingleOutputDto<ReadRoomTypeOutputDto> { Data = source };
        }
        #endregion

        /// <summary>
        /// 添加房间类型
        /// </summary>
        /// <param name="roomType"></param>
        /// <returns></returns>
        public BaseResponse InsertRoomType(CreateRoomTypeInputDto roomType)
        {
            try
            {
                var existRoomType = roomTypeRepository.IsAny(a => a.RoomTypeId == roomType.RoomTypeId);
                if (existRoomType)
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("This room type already exists.", "房间类型已存在。"), Code = BusinessStatusCode.InternalServerError };
                roomTypeRepository.Insert(new RoomType
                {
                    RoomTypeId = roomType.RoomTypeId,
                    RoomTypeName = roomType.RoomTypeName,
                    RoomRent = roomType.RoomRent,
                    RoomDeposit = roomType.RoomDeposit,
                    PricingItemsJson = RoomPricingHelper.SerializeAdditionalPricingItems(roomType.PricingItems),
                    IsDelete = roomType.IsDelete ?? 0,
                    DataInsUsr = roomType.DataInsUsr,
                    DataInsDate = roomType.DataInsDate
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting room type");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 更新房间类型
        /// </summary>
        /// <param name="roomType"></param>
        /// <returns></returns>
        public BaseResponse UpdateRoomType(UpdateRoomTypeInputDto roomType)
        {
            try
            {
                var targetRoomType = roomTypeRepository.GetFirst(a => a.Id == (roomType.Id ?? 0));
                if (targetRoomType == null)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Room Type Information Not Found", "房间类型信息未找到")
                    };
                }

                targetRoomType.RoomTypeId = roomType.RoomTypeId;
                targetRoomType.RoomTypeName = roomType.RoomTypeName;
                targetRoomType.RoomRent = roomType.RoomRent;
                targetRoomType.RoomDeposit = roomType.RoomDeposit;
                targetRoomType.PricingItemsJson = roomType.PricingItems == null
                    ? targetRoomType.PricingItemsJson
                    : RoomPricingHelper.SerializeAdditionalPricingItems(roomType.PricingItems);
                targetRoomType.IsDelete = roomType.IsDelete;
                targetRoomType.DataChgUsr = roomType.DataChgUsr;
                targetRoomType.DataChgDate = roomType.DataChgDate;
                targetRoomType.RowVersion = roomType.RowVersion ?? 0;

                var result = roomTypeRepository.Update(targetRoomType);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating room type");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除房间类型
        /// </summary>
        /// <param name="roomType"></param>
        /// <returns></returns>
        public BaseResponse DeleteRoomType(DeleteRoomTypeInputDto roomType)
        {
            try
            {
                if (roomType?.DelIds == null || !roomType.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(roomType);
                var roomTypes = roomTypeRepository.GetList(a => delIds.Contains(a.Id));

                if (!roomTypes.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Room Type Information Not Found", "房间类型信息未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(roomType, roomTypes, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                // 检查是否有房间关联到这些房间类型
                var roomTypeIds = roomTypes.Select(rt => rt.RoomTypeId).ToList();
                var associatedRooms = roomRepository.IsAny(r => roomTypeIds.Contains(r.RoomTypeId) && r.IsDelete != 1);
                if (associatedRooms)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Cannot delete room type because there are rooms associated with it.", "无法删除房间类型，因为有房间与之关联。")
                    };
                }

                var result = roomTypeRepository.SoftDeleteRange(roomTypes);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting room type");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        private static ReadRoomTypeOutputDto MapRoomTypeToOutput(RoomType source)
        {
            if (source == null)
            {
                return new ReadRoomTypeOutputDto
                {
                    PricingItems = new List<RoomTypePricingItemDto>()
                };
            }

            return new ReadRoomTypeOutputDto
            {
                Id = source.Id,
                RoomTypeId = source.RoomTypeId,
                RoomTypeName = source.RoomTypeName,
                RoomRent = source.RoomRent,
                RoomDeposit = source.RoomDeposit,
                PricingItems = RoomPricingHelper.BuildPricingItems(source.RoomRent, source.RoomDeposit, source.PricingItemsJson),
                DataInsUsr = source.DataInsUsr,
                DataInsDate = source.DataInsDate,
                DataChgUsr = source.DataChgUsr,
                DataChgDate = source.DataChgDate,
                RowVersion = source.RowVersion,
                IsDelete = source.IsDelete
            };
        }
    }
}
