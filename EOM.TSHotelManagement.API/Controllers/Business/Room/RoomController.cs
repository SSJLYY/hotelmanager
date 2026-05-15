using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 房间信息控制器
    /// </summary>
    [BusinessOperationAudit]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService roomService;

        public RoomController(IRoomService roomService)
        {
            this.roomService = roomService;
        }

        /// <summary>
        /// 根据房间状态获取相应状态的房间信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.srbrs")]
        [HttpGet]
        public ListOutputDto<ReadRoomOutputDto> SelectRoomByRoomState([FromQuery] ReadRoomInputDto inputDto)
        {
            return roomService.SelectRoomByRoomState(inputDto);
        }

        /// <summary>
        /// 根据房间状态来查询可使用的房间
        /// </summary>
        /// <returns></returns>
        [RequirePermission("roommanagement.scura")]
        [HttpGet]
        public ListOutputDto<ReadRoomOutputDto> SelectCanUseRoomAll()
        {
            return roomService.SelectCanUseRoomAll();
        }

        /// <summary>
        /// 获取所有房间信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("roommanagement.sra")]
        [HttpGet]
        public ListOutputDto<ReadRoomOutputDto> SelectRoomAll([FromQuery] ReadRoomInputDto readRoomInputDto)
        {
            return roomService.SelectRoomAll(readRoomInputDto);
        }

        /// <summary>
        /// 获取房间分区的信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.srbtn")]
        [HttpGet]
        public ListOutputDto<ReadRoomOutputDto> SelectRoomByTypeName([FromQuery] ReadRoomInputDto inputDto)
        {
            return roomService.SelectRoomByTypeName(inputDto);
        }

        /// <summary>
        /// 根据房间编号查询房间信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.srbrn")]
        [HttpGet]
        public SingleOutputDto<ReadRoomOutputDto> SelectRoomByRoomNo([FromQuery] ReadRoomInputDto inputDto)
        {
            return roomService.SelectRoomByRoomNo(inputDto);
        }

        /// <summary>
        /// 根据房间编号查询截止到今天住了多少天
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.dbrn")]
        [HttpGet]
        public SingleOutputDto<ReadRoomOutputDto> DayByRoomNo([FromQuery] ReadRoomInputDto inputDto)
        {
            return roomService.DayByRoomNo(inputDto);
        }

        /// <summary>
        /// 根据房间编号修改房间信息（入住）
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.uri")]
        [HttpPost]
        public BaseResponse UpdateRoomInfo([FromBody] UpdateRoomInputDto inputDto)
        {
            return roomService.UpdateRoomInfo(inputDto);
        }

        /// <summary>
        /// 根据房间编号修改房间信息（预约）
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.uriwr")]
        [HttpPost]
        public BaseResponse UpdateRoomInfoWithReser([FromBody] UpdateRoomInputDto inputDto)
        {
            return roomService.UpdateRoomInfoWithReser(inputDto);
        }

        /// <summary>
        /// 查询可入住房间数量
        /// </summary>
        /// <returns></returns>
        [RequirePermission("roommanagement.scurabrs")]
        [HttpGet]
        public SingleOutputDto<ReadRoomOutputDto> SelectCanUseRoomAllByRoomState()
        {
            return roomService.SelectCanUseRoomAllByRoomState();
        }

        /// <summary>
        /// 查询已入住房间数量
        /// </summary>
        /// <returns></returns>
        [RequirePermission("roommanagement.snurabrs")]
        [HttpGet]
        public SingleOutputDto<ReadRoomOutputDto> SelectNotUseRoomAllByRoomState()
        {
            return roomService.SelectNotUseRoomAllByRoomState();
        }

        /// <summary>
        /// 根据房间编号查询房间价格
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.srbrp")]
        [HttpGet]
        public object SelectRoomByRoomPrice([FromQuery] ReadRoomInputDto inputDto)
        {
            return roomService.SelectRoomByRoomPrice(inputDto);
        }

        [RequirePermission("roommanagement.srbrp")]
        [HttpGet]
        public SingleOutputDto<ReadRoomPricingOutputDto> SelectRoomPricingOptions([FromQuery] ReadRoomInputDto inputDto)
        {
            return roomService.SelectRoomPricingOptions(inputDto);
        }

        /// <summary>
        /// 查询脏房数量
        /// </summary>
        /// <returns></returns>
        [RequirePermission("roommanagement.sncrabrs")]
        [HttpGet]
        public SingleOutputDto<ReadRoomOutputDto> SelectNotClearRoomAllByRoomState()
        {
            return roomService.SelectNotClearRoomAllByRoomState();
        }

        /// <summary>
        /// 查询维修房数量
        /// </summary>
        /// <returns></returns>
        [RequirePermission("roommanagement.sfrabrs")]
        [HttpGet]
        public SingleOutputDto<ReadRoomOutputDto> SelectFixingRoomAllByRoomState()
        {
            return roomService.SelectFixingRoomAllByRoomState();
        }

        /// <summary>
        /// 查询预约房数量
        /// </summary>
        /// <returns></returns>
        [RequirePermission("roommanagement.srrabrs")]
        [HttpGet]
        public SingleOutputDto<ReadRoomOutputDto> SelectReservedRoomAllByRoomState()
        {
            return roomService.SelectReservedRoomAllByRoomState();
        }

        /// <summary>
        /// 根据房间编号更改房间状态
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.ursbrn")]
        [HttpPost]
        public BaseResponse UpdateRoomStateByRoomNo([FromBody] UpdateRoomInputDto inputDto)
        {
            return roomService.UpdateRoomStateByRoomNo(inputDto);
        }

        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.insertroom")]
        [HttpPost]
        public BaseResponse InsertRoom([FromBody] CreateRoomInputDto inputDto)
        {
            return roomService.InsertRoom(inputDto);
        }

        /// <summary>
        /// 更新房间
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.updateroom")]
        [HttpPost]
        public BaseResponse UpdateRoom([FromBody] UpdateRoomInputDto inputDto)
        {
            return roomService.UpdateRoom(inputDto);
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.deleteroom")]
        [HttpPost]
        public BaseResponse DeleteRoom([FromBody] DeleteRoomInputDto inputDto)
        {
            return roomService.DeleteRoom(inputDto);
        }

        /// <summary>
        /// 转房操作
        /// </summary>
        /// <param name="transferRoomDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.tr")]
        [HttpPost]
        public BaseResponse TransferRoom([FromBody] TransferRoomDto transferRoomDto)
        {
            return roomService.TransferRoom(transferRoomDto);
        }

        /// <summary>
        /// 退房操作
        /// </summary>
        /// <param name="checkoutRoomDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.cr")]
        [HttpPost]
        public BaseResponse CheckoutRoom([FromBody] CheckoutRoomDto checkoutRoomDto)
        {
            return roomService.CheckoutRoom(checkoutRoomDto);
        }

        /// <summary>
        /// 根据预约信息办理入住
        /// </summary>
        /// <param name="checkinRoomByReservationDto"></param>
        /// <returns></returns>
        [RequirePermission("roommanagement.crbr")]
        [HttpPost]
        public BaseResponse CheckinRoomByReservation([FromBody] CheckinRoomByReservationDto checkinRoomByReservationDto)
        {
            return roomService.CheckinRoomByReservation(checkinRoomByReservationDto);
        }
    }
}
