using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 员工履历控制器
    /// </summary>
    public class EmployeeHistoryController : ControllerBase
    {
        private readonly IEmployeeHistoryService workerHistoryService;

        public EmployeeHistoryController(IEmployeeHistoryService workerHistoryService)
        {
            this.workerHistoryService = workerHistoryService;
        }

        /// <summary>
        /// 根据工号添加员工履历
        /// </summary>
        /// <param name="workerHistory"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.ahbei")]
        [HttpPost]
        public BaseResponse AddHistoryByEmployeeId([FromBody] CreateEmployeeHistoryInputDto workerHistory)
        {
            return workerHistoryService.AddHistoryByEmployeeId(workerHistory);
        }

        /// <summary>
        /// 根据工号更新员工履历
        /// </summary>
        /// <param name="workerHistory"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.update")]
        [HttpPost]
        public BaseResponse UpdateHistoryByEmployeeId([FromBody] UpdateEmployeeHistoryInputDto workerHistory)
        {
            return workerHistoryService.UpdateHistoryByEmployeeId(workerHistory);
        }

        /// <summary>
        /// 根据工号查询履历信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.shbei")]
        [HttpGet]
        public ListOutputDto<ReadEmployeeHistoryOutputDto> SelectHistoryByEmployeeId([FromQuery] ReadEmployeeHistoryInputDto inputDto)
        {
            return workerHistoryService.SelectHistoryByEmployeeId(inputDto);
        }
    }
}

