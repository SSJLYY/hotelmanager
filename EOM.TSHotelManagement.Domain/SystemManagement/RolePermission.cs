using SqlSugar;
namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 角色权限关联表 (Role-Permission Mapping)
    /// </summary>
    [SugarTable("role_permission", "角色权限关联表 (Role-Permission Mapping)")]
    public class RolePermission : AuditEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 角色编码（关联角色表） (Role Code)
        /// </summary>
        [SugarColumn(
            ColumnName = "role_number",
            ColumnDescription = "关联角色编码 (Linked Role Code)",
            IsNullable = false,
            Length = 128,
            IndexGroupNameList = new[] { "IX_role_number" }
        )]
        public string RoleNumber { get; set; } = null!;

        /// <summary>
        /// 权限编码（关联权限表） (Permission Code)
        /// </summary>
        [SugarColumn(
            ColumnName = "permission_number",
            ColumnDescription = "关联权限编码 (Linked Permission Code)",
            IsNullable = true,
            Length = 128,
            IndexGroupNameList = new[] { "IX_permission_number" }
        )]
        public string? PermissionNumber { get; set; }

        /// <summary>
        /// 菜单主键（关联菜单表） (Menu Id)
        /// </summary>
        [SugarColumn(
            ColumnName = "menu_id",
            ColumnDescription = "关联菜单主键 (Linked Menu Id)",
            IsNullable = true,
            IndexGroupNameList = new[] { "IX_menu_id" }
        )]
        public int? MenuId { get; set; }
    }

}
