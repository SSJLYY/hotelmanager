using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 系统权限定义表 (Permission Definition)
    /// </summary>
    [SugarTable("permission", "系统权限定义表 (Permission Definition)")]
    public class Permission : AuditEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 权限编码（唯一，如 system:role:list）
        /// </summary>
        [SugarColumn(
            ColumnName = "permission_number",
            ColumnDescription = "权限编码（唯一，如 system:role:list）",
            Length = 128,
            IsNullable = false,
            UniqueGroupNameList = new[] { "UK_permission_number" }
        )]
        public string PermissionNumber { get; set; } = null!;

        /// <summary>
        /// 权限名称（中文显示）
        /// </summary>
        [SugarColumn(
            ColumnName = "permission_name",
            ColumnDescription = "权限名称（中文显示）",
            Length = 200,
            IsNullable = false
        )]
        public string PermissionName { get; set; } = null!;

        /// <summary>
        /// 所属模块（如 AdminManager / RoomManager 等）
        /// </summary>
        [SugarColumn(
            ColumnName = "module",
            ColumnDescription = "所属模块（如 AdminManager / RoomManager 等）",
            Length = 128,
            IsNullable = false
        )]
        public string Module { get; set; } = null!;

        /// <summary>
        /// 权限说明
        /// </summary>
        [SugarColumn(
            ColumnName = "permission_description",
            ColumnDescription = "权限说明",
            Length = 500,
            IsNullable = true
        )]
        public string? Description { get; set; }

        /// <summary>
        /// 关联菜单Key（用于菜单过滤，非必填）
        /// </summary>
        [SugarColumn(
            ColumnName = "menu_key",
            ColumnDescription = "关联菜单Key（用于菜单过滤，非必填）",
            Length = 256,
            IsNullable = true
        )]
        public string? MenuKey { get; set; }

        /// <summary>
        /// 父级权限编码（可选，用于树形权限）
        /// </summary>
        [SugarColumn(
            ColumnName = "parent_number",
            ColumnDescription = "父级权限编码（可选，用于树形权限）",
            Length = 128,
            IsNullable = true
        )]
        public string? ParentNumber { get; set; }
    }
}
