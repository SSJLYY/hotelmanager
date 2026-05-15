using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 消费信息控制器
    /// </summary>
    [BusinessOperationAudit]
    public class SpendController : ControllerBase
    {
        private readonly ISpendService spendService;

        public SpendController(ISpendService spendService)
        {
            this.spendService = spendService;
        }

        /// <summary>
        /// 根据房间编号查询消费信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("customerspend.ssbrn")]
        [HttpGet]
        public ListOutputDto<ReadSpendOutputDto> SelectSpendByRoomNo([FromQuery] ReadSpendInputDto inputDto)
        {
            return spendService.SelectSpendByRoomNo(inputDto);
        }

        /// <summary>
        /// 根据客户编号查询历史消费信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("customerspend.shsia")]
        [HttpGet]
        public ListOutputDto<ReadSpendOutputDto> SeletHistorySpendInfoAll([FromQuery] ReadSpendInputDto inputDto)
        {
            return spendService.SeletHistorySpendInfoAll(inputDto);
        }

        /// <summary>
        /// 查询消费的所有信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("customerspend.ssia")]
        [HttpGet]
        public ListOutputDto<ReadSpendOutputDto> SelectSpendInfoAll([FromQuery] ReadSpendInputDto readSpendInputDto)
        {
            return spendService.SelectSpendInfoAll(readSpendInputDto);
        }

        /// <summary>
        /// 根据房间编号、入住时间到当前时间查询消费总金额
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("customerspend.sca")]
        [HttpGet]
        public SingleOutputDto<ReadSpendInputDto> SumConsumptionAmount([FromQuery] ReadSpendInputDto inputDto)
        {
            return spendService.SumConsumptionAmount(inputDto);
        }

        /// <summary>
        /// 撤回客户消费信息
        /// </summary>
        /// <param name="undoCustomerSpendInputDto"></param>
        /// <returns></returns>
        [RequirePermission("customerspend.ucs")]
        [HttpPost]
        public BaseResponse UndoCustomerSpend([FromBody] UndoCustomerSpendInputDto undoCustomerSpendInputDto)
        {
            return spendService.UndoCustomerSpend(undoCustomerSpendInputDto);
        }

        /// <summary>
        /// 添加客户消费信息
        /// </summary>
        /// <param name="addCustomerSpendInputDto"></param>
        /// <returns></returns>
        [RequirePermission("customerspend.acs")]
        [HttpPost]
        public BaseResponse AddCustomerSpend([FromBody] AddCustomerSpendInputDto addCustomerSpendInputDto)
        {
            return spendService.AddCustomerSpend(addCustomerSpendInputDto);
        }

        /// <summary>
        /// 更新消费信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("customerspend.usi")]
        [HttpPost]
        public BaseResponse UpdSpendInfo([FromBody] UpdateSpendInputDto inputDto)
        {
            return spendService.UpdSpendInfo(inputDto);
        }
    }
}
