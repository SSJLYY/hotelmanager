namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 保存收藏夹响应 DTO
    /// </summary>
    public class SaveFavoriteCollectionOutputDto
    {
        /// <summary>
        /// 是否保存成功
        /// </summary>
        public bool Saved { get; set; }

        /// <summary>
        /// 保存后的收藏数量
        /// </summary>
        public int RouteCount { get; set; }

        /// <summary>
        /// 最终生效的更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 保存成功后的最新版本号
        /// </summary>
        public long RowVersion { get; set; }
    }
}