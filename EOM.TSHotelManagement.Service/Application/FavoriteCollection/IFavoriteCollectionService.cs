using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 收藏夹服务接口
    /// </summary>
    public interface IFavoriteCollectionService
    {
        /// <summary>
        /// 保存当前用户的收藏夹快照
        /// </summary>
        /// <param name="input">收藏夹保存请求</param>
        /// <returns>保存结果</returns>
        SingleOutputDto<SaveFavoriteCollectionOutputDto> SaveFavoriteCollection(SaveFavoriteCollectionInputDto input);

        /// <summary>
        /// 获取当前用户的收藏夹快照
        /// </summary>
        /// <returns>收藏夹读取结果</returns>
        SingleOutputDto<ReadFavoriteCollectionOutputDto> GetFavoriteCollection();
    }
}
