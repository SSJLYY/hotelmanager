using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("dashboard.rs")]
        [HttpGet]
        public SingleOutputDto<RoomStatisticsOutputDto> RoomStatistics()
        {
            return dashboardService.RoomStatistics();
        }

        /// <summary>
        /// 获取业务统计信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("dashboard.bs")]
        [HttpGet]
        public SingleOutputDto<BusinessStatisticsOutputDto> BusinessStatistics()
        {
            return dashboardService.BusinessStatistics();
        }

        /// <summary>
        /// 获取后勤统计信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("dashboard.ls")]
        [HttpGet]
        public SingleOutputDto<LogisticsDataOutputDto> LogisticsStatistics()
        {
            return dashboardService.LogisticsStatistics();
        }

        /// <summary>
        /// 获取人事统计信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("dashboard.hrs")]
        [HttpGet]
        public SingleOutputDto<HumanResourcesOutputDto> HumanResourcesStatistics()
        {
            return dashboardService.HumanResourcesStatistics();
        }
    }
}
