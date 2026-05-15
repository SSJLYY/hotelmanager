using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 水电信息控制器
    /// </summary>
    [BusinessOperationAudit]
    public class EnergyManagementController : ControllerBase
    {
        /// <summary>
        /// 水电信息服务
        /// </summary>
        private readonly IEnergyManagementService hydroelectricPowerService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hydroelectricPowerService"></param>
        public EnergyManagementController(IEnergyManagementService hydroelectricPowerService)
        {
            this.hydroelectricPowerService = hydroelectricPowerService;
        }

        /// <summary>
        /// 根据条件查询水电费信息
        /// 替换了 SelectWtiInfoByRoomNo, SelectWtiInfoByRoomNoAndTime, ListWtiInfoByRoomNo, SelectWtiInfoAll
        /// </summary>
        /// <param name="readEnergyManagementInputDto">Dto</param>
        /// <returns>符合条件的水电费信息列表</returns>
        [RequirePermission("hydroelectricinformation.semi")]
        [HttpGet]
        public ListOutputDto<ReadEnergyManagementOutputDto> SelectEnergyManagementInfo([FromQuery] ReadEnergyManagementInputDto readEnergyManagementInputDto)
        {
            return this.hydroelectricPowerService.SelectEnergyManagementInfo(readEnergyManagementInputDto);
        }

        /// <summary>
        /// 添加水电费信息
        /// 替换了 InsertWtiInfo
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        [RequirePermission("hydroelectricinformation.iemi")]
        [HttpPost]
        public BaseResponse InsertEnergyManagementInfo([FromBody] CreateEnergyManagementInputDto w)
        {
            return this.hydroelectricPowerService.InsertEnergyManagementInfo(w);
        }

        /// <summary>
        /// 修改水电费信息
        /// 替换了 UpdateWtiInfo 和 UpdateWtiInfoByRoomNoAndDateTime
        /// </summary>
        /// <param name="w">包含要修改的数据，以及WtiNo作为查询条件</param>
        /// <returns></returns>
        [RequirePermission("hydroelectricinformation.uemi")]
        [HttpPost]
        public BaseResponse UpdateEnergyManagementInfo([FromBody] UpdateEnergyManagementInputDto w)
        {
            return this.hydroelectricPowerService.UpdateEnergyManagementInfo(w);
        }


        /// <summary>
        /// 根据房间编号、使用时间删除水电费信息
        /// 替换了 DeleteWtiInfoByRoomNoAndDateTime
        /// </summary>
        /// <param name="deleteEnergyManagementInputDto"></param>
        /// <returns></returns>
        [RequirePermission("hydroelectricinformation.demi")]
        [HttpPost]
        public BaseResponse DeleteEnergyManagementInfo([FromBody] DeleteEnergyManagementInputDto deleteEnergyManagementInputDto)
        {
            return this.hydroelectricPowerService.DeleteEnergyManagementInfo(deleteEnergyManagementInputDto);
        }
    }
}
