using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    public interface INewsService
    {
        /// <summary>
        /// 查询新闻列表
        /// </summary>
        /// <param name="readNewsInputDto"></param>
        /// <returns></returns>
        ListOutputDto<ReadNewsOuputDto> SelectNews(ReadNewsInputDto readNewsInputDto);
        /// <summary>
        /// 查询单条新闻
        /// </summary>
        /// <param name="readNewsInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadNewsOuputDto> News(ReadNewsInputDto readNewsInputDto);
        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="addNewsInputDto"></param>
        /// <returns></returns>
        BaseResponse AddNews(AddNewsInputDto addNewsInputDto);
        /// <summary>
        /// 更新新闻
        /// </summary>
        /// <param name="updateNewsInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdateNews(UpdateNewsInputDto updateNewsInputDto);
        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="deleteNewsInputDto"></param>
        /// <returns></returns>
        BaseResponse DeleteNews(DeleteNewsInputDto deleteNewsInputDto);
    }
}
