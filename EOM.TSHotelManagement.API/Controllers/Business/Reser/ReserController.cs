using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 预约信息控制器
    /// </summary>
    [BusinessOperationAudit]
    public class ReserController : ControllerBase
    {
        /// <summary>
        /// 预约信息
        /// </summary>
        private readonly IReserService reserService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reserService"></param>
        public ReserController(IReserService reserService)
        {
            this.reserService = reserService;
        }

        /// <summary>
        /// 获取所有预约信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("resermanagement.sra")]
        [HttpGet]
        public ListOutputDto<ReadReserOutputDto> SelectReserAll(ReadReserInputDto readReserInputDto)
        {
            return reserService.SelectReserAll(readReserInputDto);
        }

        /// <summary>
        /// 根据房间编号获取预约信息
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        [RequirePermission("resermanagement.sribrn")]
        [HttpGet]
        public SingleOutputDto<ReadReserOutputDto> SelectReserInfoByRoomNo([FromQuery] ReadReserInputDto readReserInputDto)
        {
            return reserService.SelectReserInfoByRoomNo(readReserInputDto);
        }

        /// <summary>
        /// 删除预约信息
        /// </summary>
        /// <param name="reser"></param>
        /// <returns></returns>
        [RequirePermission("resermanagement.dri")]
        [HttpPost]
        public BaseResponse DeleteReserInfo([FromBody] DeleteReserInputDto reser)
        {
            return reserService.DeleteReserInfo(reser);
        }

        /// <summary>
        /// 更新预约信息
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [RequirePermission("resermanagement.uri")]
        [HttpPost]
        public BaseResponse UpdateReserInfo([FromBody] UpdateReserInputDto r)
        {
            return reserService.UpdateReserInfo(r);
        }

        /// <summary>
        /// 添加预约信息
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [RequirePermission("resermanagement.iri")]
        [HttpPost]
        public BaseResponse InserReserInfo([FromBody] CreateReserInputDto r)
        {
            return reserService.InserReserInfo(r);
        }

        /// <summary>
        /// 查询所有预约类型
        /// </summary>
        /// <returns></returns>
        [RequirePermission("resermanagement.srta")]
        [HttpGet]
        public ListOutputDto<EnumDto> SelectReserTypeAll()
        {
            return reserService.SelectReserTypeAll();
        }

    }
}
