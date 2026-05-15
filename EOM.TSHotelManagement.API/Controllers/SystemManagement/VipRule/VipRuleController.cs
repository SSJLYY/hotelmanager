using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 会员规则控制器
    /// </summary>
    public class VipRuleController : ControllerBase
    {
        private readonly IVipRuleAppService vipRuleAppService;

        public VipRuleController(IVipRuleAppService vipRuleAppService)
        {
            this.vipRuleAppService = vipRuleAppService;
        }

        /// <summary>
        /// 查询会员等级规则列表
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("viplevel.svrlist")]
        [HttpGet]
        public ListOutputDto<ReadVipLevelRuleOutputDto> SelectVipRuleList([FromQuery] ReadVipLevelRuleInputDto inputDto)
        {
            return vipRuleAppService.SelectVipRuleList(inputDto);
        }

        /// <summary>
        /// 查询会员等级规则
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("viplevel.svr")]
        [HttpGet]
        public SingleOutputDto<ReadVipLevelRuleOutputDto> SelectVipRule([FromQuery] ReadVipLevelRuleInputDto inputDto)
        {
            return vipRuleAppService.SelectVipRule(inputDto);
        }

        /// <summary>
        /// 添加会员等级规则
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("viplevel.addviprule")]
        [HttpPost]
        public BaseResponse AddVipRule([FromBody] CreateVipLevelRuleInputDto inputDto)
        {
            return vipRuleAppService.AddVipRule(inputDto);
        }

        /// <summary>
        /// 删除会员等级规则
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("viplevel.delviprule")]
        [HttpPost]
        public BaseResponse DelVipRule([FromBody] DeleteVipLevelRuleInputDto inputDto)
        {
            return vipRuleAppService.DelVipRule(inputDto);
        }

        /// <summary>
        /// 更新会员等级规则
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("viplevel.updviprule")]
        [HttpPost]
        public BaseResponse UpdVipRule([FromBody] UpdateVipLevelRuleInputDto inputDto)
        {
            return vipRuleAppService.UpdVipRule(inputDto);
        }
    }
}


