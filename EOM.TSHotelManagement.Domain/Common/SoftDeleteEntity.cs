using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Domain;

public class SoftDeleteEntity : AuditEntity
{
    /// <summary>
    /// 删除标识
    /// </summary>
    [SqlSugar.SugarColumn(ColumnName = "delete_mk", Length = 11, IsNullable = false, DefaultValue = "0")]
    public int? IsDelete { get; set; } = 0;
}
