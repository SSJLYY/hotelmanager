using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    public interface IDashboardService
    {
        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<RoomStatisticsOutputDto> RoomStatistics();

        /// <summary>
        /// 获取业务统计信息
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<BusinessStatisticsOutputDto> BusinessStatistics();

        /// <summary>
        /// 获取后勤统计信息
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<LogisticsDataOutputDto> LogisticsStatistics();

        /// <summary>
        /// 获取人事统计信息
        /// </summary>
        /// <returns></returns>
        SingleOutputDto<HumanResourcesOutputDto> HumanResourcesStatistics();
    }
}
