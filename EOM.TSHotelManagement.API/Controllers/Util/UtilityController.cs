using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 工具类控制器
    /// </summary>
    public class UtilityController : ControllerBase
    {
        private readonly IUtilService utilService;

        public UtilityController(IUtilService utilService)
        {
            this.utilService = utilService;
        }

        /// <summary>
        /// 查询身份证号码
        /// </summary>
        /// <param name="readCardCodeInputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public SingleOutputDto<ReadCardCodeOutputDto> SelectCardCode([FromBody] ReadCardCodeInputDto readCardCodeInputDto)
        {
            return utilService.SelectCardCode(readCardCodeInputDto);
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse AddLog([FromBody] CreateOperationLogInputDto opr)
        {
            return utilService.AddLog(opr);
        }

        /// <summary>
        /// 查询所有操作日志
        /// </summary>
        /// <returns></returns>
        [RequirePermission("operationlog.view")]
        [HttpGet]
        public ListOutputDto<ReadOperationLogOutputDto> SelectOperationlogAll([FromQuery] ReadOperationLogInputDto readOperationLogInputDto)
        {
            return utilService.SelectOperationlogAll(readOperationLogInputDto);
        }

        /// <summary>
        /// 查询所有请求日志
        /// </summary>
        /// <param name="readRequestLogInputDto"></param>
        /// <returns></returns>
        [RequirePermission("requestlog.view")]
        [HttpGet]
        public ListOutputDto<ReadRequestLogOutputDto> SelectRequestlogAll([FromQuery] ReadRequestLogInputDto readRequestLogInputDto)
        {
            return utilService.SelectRequestlogAll(readRequestLogInputDto);
        }

        /// <summary>
        /// 删除时间范围的请求日志
        /// </summary>
        /// <param name="readRequestLogInputDto"></param>
        /// <returns></returns>
        [RequirePermission("requestlog.delete")]
        [HttpPost]
        public BaseResponse DeleteRequestlogByRange([FromBody] ReadRequestLogInputDto readRequestLogInputDto)
        {
            return utilService.DeleteRequestlogByRange(readRequestLogInputDto);
        }

        /// <summary>
        /// 删除时间范围的操作日志
        /// </summary>
        /// <param name="readOperationLogInputDto"></param>
        /// <returns></returns>
        [RequirePermission("operationlog.delete")]
        [HttpPost]
        public BaseResponse DeleteOperationlogByRange([FromBody] ReadOperationLogInputDto readOperationLogInputDto)
        {
            return utilService.DeleteOperationlogByRange(readOperationLogInputDto);
        }

        /// <summary>
        /// 删除操作日志
        /// </summary>
        /// <param name="deleteOperationLogInputDto"></param>
        /// <returns></returns>
        [RequirePermission("operationlog.delete")]
        [HttpPost]
        public BaseResponse DeleteOperationlog([FromBody] DeleteOperationLogInputDto deleteOperationLogInputDto)
        {
            return utilService.DeleteOperationlog(deleteOperationLogInputDto);
        }
    }
}
