using System;

namespace EOM.TSHotelManagement.Domain
{
    public class BaseEntity
    {
        /// <summary>
        /// 行版本（乐观锁）
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "row_version", IsNullable = false, DefaultValue = "1")]
        public long RowVersion { get; set; } = 1;
        /// <summary>
        /// Token
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string? UserToken { get; set; }
    }
}
