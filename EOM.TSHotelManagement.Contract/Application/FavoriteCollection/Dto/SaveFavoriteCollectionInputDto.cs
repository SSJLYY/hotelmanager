using System.ComponentModel.DataAnnotations;

namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 保存收藏夹请求 DTO
    /// </summary>
    public class SaveFavoriteCollectionInputDto
    {
        /// <summary>
        /// 乐观锁版本号，更新已有快照时必填
        /// </summary>
        public long? RowVersion { get; set; }

        /// <summary>
        /// 登录类型，前端可能传 admin 或 employee
        /// </summary>
        [MaxLength(32, ErrorMessage = "LoginType length cannot exceed 32 characters.")]
        public string? LoginType { get; set; }

        /// <summary>
        /// 前端当前账号
        /// </summary>
        [MaxLength(128, ErrorMessage = "Account length cannot exceed 128 characters.")]
        public string? Account { get; set; }

        /// <summary>
        /// 当前完整收藏路由列表
        /// </summary>
        [Required(ErrorMessage = "FavoriteRoutes is required.")]
        public List<string> FavoriteRoutes { get; set; } = new();

        /// <summary>
        /// 前端最后一次修改收藏夹的时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 触发来源，例如 logout、pagehide、beforeunload、manual
        /// </summary>
        [MaxLength(32, ErrorMessage = "TriggeredBy length cannot exceed 32 characters.")]
        public string? TriggeredBy { get; set; }
    }
}