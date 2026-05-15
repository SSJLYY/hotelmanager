using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi
{
    [ApiExplorerSettings(GroupName = "v1_Mobile")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService newsService;
        public NewsController(INewsService newsService)
        {
            this.newsService = newsService;
        }
        /// <summary>
        /// 查询新闻列表
        /// </summary>
        /// <param name="readNewsInputDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ListOutputDto<ReadNewsOuputDto> SelectNews([FromQuery] ReadNewsInputDto readNewsInputDto)
        {
            return newsService.SelectNews(readNewsInputDto);
        }
        /// <summary>
        /// 查询单条新闻
        /// </summary>
        /// <param name="readNewsInputDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public SingleOutputDto<ReadNewsOuputDto> News([FromQuery] ReadNewsInputDto readNewsInputDto)
        {
            return newsService.News(readNewsInputDto);
        }
        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="addNewsInputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse AddNews([FromBody] AddNewsInputDto addNewsInputDto)
        {
            return newsService.AddNews(addNewsInputDto);
        }
        /// <summary>
        /// 更新新闻
        /// </summary>
        /// <param name="updateNewsInputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse UpdateNews([FromBody] UpdateNewsInputDto updateNewsInputDto)
        {
            return newsService.UpdateNews(updateNewsInputDto);
        }
        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="deleteNewsInputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse DeleteNews([FromBody] DeleteNewsInputDto deleteNewsInputDto)
        {
            return newsService.DeleteNews(deleteNewsInputDto);
        }
    }
}
