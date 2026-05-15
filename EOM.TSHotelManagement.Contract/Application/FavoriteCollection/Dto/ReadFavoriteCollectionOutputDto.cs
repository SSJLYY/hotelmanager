namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 读取收藏夹响应 DTO
    /// </summary>
    public class ReadFavoriteCollectionOutputDto
    {
        /// <summary>
        /// 收藏路由列表
        /// </summary>
        public List<string> FavoriteRoutes { get; set; } = new();

        /// <summary>
        /// 收藏夹最后更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 当前快照版本号
        /// </summary>
        public long? RowVersion { get; set; }
    }
}