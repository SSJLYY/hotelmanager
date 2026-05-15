using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 收藏夹持久化接口
    /// </summary>
    public class FavoriteCollectionController : ControllerBase
    {
        private readonly IFavoriteCollectionService _favoriteCollectionService;

        /// <summary>
        /// 构造收藏夹控制器
        /// </summary>
        /// <param name="favoriteCollectionService">收藏夹服务</param>
        public FavoriteCollectionController(IFavoriteCollectionService favoriteCollectionService)
        {
            _favoriteCollectionService = favoriteCollectionService;
        }

        /// <summary>
        /// 保存当前登录用户的收藏夹快照
        /// </summary>
        /// <param name="input">收藏夹保存请求</param>
        /// <returns>保存结果</returns>
        [HttpPost]
        public SingleOutputDto<SaveFavoriteCollectionOutputDto> SaveFavoriteCollection([FromBody] SaveFavoriteCollectionInputDto input)
        {
            return _favoriteCollectionService.SaveFavoriteCollection(input);
        }

        /// <summary>
        /// 获取当前登录用户的收藏夹快照
        /// </summary>
        /// <returns>收藏夹读取结果</returns>
        [HttpGet]
        public SingleOutputDto<ReadFavoriteCollectionOutputDto> GetFavoriteCollection()
        {
            return _favoriteCollectionService.GetFavoriteCollection();
        }
    }
}
