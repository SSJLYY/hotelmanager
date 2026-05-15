using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    public class QuartzController : ControllerBase
    {
        private readonly IQuartzAppService _quartzAppService;

        public QuartzController(IQuartzAppService quartzAppService)
        {
            _quartzAppService = quartzAppService;
        }

        /// <summary>
        /// 查询当前已注册的Quartz任务和触发器列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ListOutputDto<ReadQuartzJobOutputDto>> SelectQuartzJobList()
        {
            return await _quartzAppService.SelectQuartzJobList();
        }
    }
}
