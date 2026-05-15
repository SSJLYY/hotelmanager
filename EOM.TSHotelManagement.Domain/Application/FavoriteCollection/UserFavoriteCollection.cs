using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 用户收藏夹快照实体
    /// </summary>
    [SugarTable("user_favorite_collection", "User favorite collection snapshot", true)]
    public class UserFavoriteCollection : AuditEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "Id")]
        public int Id { get; set; }

        /// <summary>
        /// JWT 中的用户编号
        /// </summary>
        [SugarColumn(
            ColumnName = "user_number",
            Length = 128,
            IsNullable = false,
            UniqueGroupNameList = new[] { "UK_user_favorite_collection_user_number" },
            ColumnDescription = "User number resolved from JWT"
        )]
        public string UserNumber { get; set; } = string.Empty;

        /// <summary>
        /// 登录类型
        /// </summary>
        [SugarColumn(
            ColumnName = "login_type",
            Length = 32,
            IsNullable = true,
            ColumnDescription = "Login type"
        )]
        public string? LoginType { get; set; }

        /// <summary>
        /// 当前账号
        /// </summary>
        [SugarColumn(
            ColumnName = "account",
            Length = 128,
            IsNullable = true,
            ColumnDescription = "Account"
        )]
        public string? Account { get; set; }

        /// <summary>
        /// 收藏路由 JSON 快照
        /// </summary>
        [SugarColumn(
            ColumnName = "favorite_routes_json",
            ColumnDataType = "text",
            IsNullable = false,
            ColumnDescription = "Favorite routes JSON snapshot"
        )]
        public string FavoriteRoutesJson { get; set; } = "[]";

        /// <summary>
        /// 收藏数量
        /// </summary>
        [SugarColumn(
            ColumnName = "route_count",
            IsNullable = false,
            DefaultValue = "0",
            ColumnDescription = "Favorite route count"
        )]
        public int RouteCount { get; set; }

        /// <summary>
        /// 业务更新时间
        /// </summary>
        [SugarColumn(
            ColumnName = "updated_at",
            IsNullable = false,
            ColumnDescription = "Snapshot updated time from client"
        )]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 最后触发来源
        /// </summary>
        [SugarColumn(
            ColumnName = "triggered_by",
            Length = 32,
            IsNullable = true,
            ColumnDescription = "Last trigger source"
        )]
        public string? TriggeredBy { get; set; }
    }
}
