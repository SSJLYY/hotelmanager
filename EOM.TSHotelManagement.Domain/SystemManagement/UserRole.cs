using SqlSugar;
namespace EOM.TSHotelManagement.Domain
{
    // <summary>
    /// 用户角色关联表 (User-Role Mapping)
    /// </summary>
    [SugarTable("user_role", "用户角色关联表 (User-Role Mapping)")]
    public class UserRole : AuditEntity
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
            IsPrimaryKey = true,
            ColumnDescription = "关联角色编码 (Linked Role Code)",
            IsNullable = false,
            Length = 128,
            IndexGroupNameList = new[] { "IX_role_number" }
        )]
        public string RoleNumber { get; set; } = null!;

        /// <summary>
        /// 用户编码（关联用户表） (User Code)
        /// </summary>
        [SugarColumn(
            ColumnName = "user_number",
            IsPrimaryKey = true,
            ColumnDescription = "关联用户编码 (Linked User Code)",
            IsNullable = false,
            Length = 128,
            IndexGroupNameList = new[] { "IX_user_number" }
        )]
        public string UserNumber { get; set; } = null!;
    }

}
