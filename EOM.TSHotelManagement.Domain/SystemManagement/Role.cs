using SqlSugar;
namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 系统角色配置表 (System Role Configuration)
    /// </summary>
    [SugarTable("role", "系统角色配置表 (System Role Configuration)")]
    public class Role : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 角色编码 (Role Code)
        /// </summary>
        [SugarColumn(
            ColumnName = "role_number",
            IsPrimaryKey = true,
            ColumnDescription = "角色唯一标识编码 (Unique Role Identifier)",
            IsNullable = false,
            Length = 128
        )]
        public string RoleNumber { get; set; } = null!;

        /// <summary>
        /// 角色名称 (Role Name)
        /// </summary>
        [SugarColumn(
            ColumnName = "role_name",
            ColumnDescription = "角色名称（如管理员/前台接待）",
            IsNullable = false,
            Length = 200
        )]
        public string RoleName { get; set; } = null!;

        /// <summary>
        /// 角色描述 (Role Description)
        /// </summary>
        [SugarColumn(
            ColumnName = "role_description",
            ColumnDescription = "角色详细权限描述 (Detailed Permissions Description)",
            IsNullable = true,
            Length = 500
        )]
        public string? RoleDescription { get; set; }
    }

}
