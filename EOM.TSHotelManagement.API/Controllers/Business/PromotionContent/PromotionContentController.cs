using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 酒店宣传联动内容控制器
    /// </summary>
    [BusinessOperationAudit]
    public class PromotionContentController : ControllerBase
    {
        /// <summary>
        /// 酒店宣传联动内容
        /// </summary>
        private readonly IPromotionContentService fontsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontsService"></param>
        public PromotionContentController(IPromotionContentService fontsService)
        {
            this.fontsService = fontsService;
        }

        /// <summary>
        /// 查询所有宣传联动内容
        /// </summary>
        /// <returns></returns>
        [RequirePermission("promotioncontent.spca")]
        [HttpGet]
        public ListOutputDto<ReadPromotionContentOutputDto> SelectPromotionContentAll([FromQuery] ReadPromotionContentInputDto readPromotionContentInputDto)
        {
            return fontsService.SelectPromotionContentAll(readPromotionContentInputDto);
        }

        /// <summary>
        /// 查询所有宣传联动内容(跑马灯)
        /// </summary>
        /// <returns></returns>
        [RequirePermission("promotioncontent.spcs")]
        [HttpGet]
        public ListOutputDto<ReadPromotionContentOutputDto> SelectPromotionContents()
        {
            return fontsService.SelectPromotionContents();
        }

        /// <summary>
        /// 添加宣传联动内容
        /// </summary>
        /// <param name="createPromotionContentInputDto"></param>
        /// <returns></returns>
        [RequirePermission("promotioncontent.apc")]
        [HttpPost]
        public BaseResponse AddPromotionContent([FromBody] CreatePromotionContentInputDto createPromotionContentInputDto)
        {
            return fontsService.AddPromotionContent(createPromotionContentInputDto);
        }

        /// <summary>
        /// 删除宣传联动内容
        /// </summary>
        /// <param name="deletePromotionContentInputDto"></param>
        /// <returns></returns>
        [RequirePermission("promotioncontent.dpc")]
        [HttpPost]
        public BaseResponse DeletePromotionContent([FromBody] DeletePromotionContentInputDto deletePromotionContentInputDto)
        {
            return fontsService.DeletePromotionContent(deletePromotionContentInputDto);
        }

        /// <summary>
        /// 更新宣传联动内容
        /// </summary>
        /// <param name="updatePromotionContentInputDto"></param>
        /// <returns></returns>
        [RequirePermission("promotioncontent.upc")]
        [HttpPost]
        public BaseResponse UpdatePromotionContent([FromBody] UpdatePromotionContentInputDto updatePromotionContentInputDto)
        {
            return fontsService.UpdatePromotionContent(updatePromotionContentInputDto);
        }
    }
}
