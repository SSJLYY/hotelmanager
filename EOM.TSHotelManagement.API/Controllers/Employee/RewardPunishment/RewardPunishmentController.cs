using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 奖惩信息控制器
    /// </summary>
    public class RewardPunishmentController : ControllerBase
    {
        /// <summary>
        /// 奖惩信息
        /// </summary>
        private readonly IRewardPunishmentService workerGoodBadService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workerGoodBadService"></param>
        public RewardPunishmentController(IRewardPunishmentService workerGoodBadService)
        {
            this.workerGoodBadService = workerGoodBadService;
        }

        /// <summary>
        /// 添加员工奖惩记录
        /// </summary>
        /// <param name="goodBad"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse AddRewardPunishment([FromBody] CreateEmployeeRewardPunishmentInputDto goodBad)
        {
            return workerGoodBadService.AddRewardPunishment(goodBad);
        }

        /// <summary>
        /// 根据工号查找所有的奖惩记录信息
        /// </summary>
        /// <param name="wn"></param>
        /// <returns></returns>
        [HttpGet]
        public ListOutputDto<ReadEmployeeRewardPunishmentOutputDto> SelectAllRewardPunishmentByEmployeeId([FromQuery] ReadEmployeeRewardPunishmentInputDto wn)
        {
            return workerGoodBadService.SelectAllRewardPunishmentByEmployeeId(wn);
        }
    }
}
