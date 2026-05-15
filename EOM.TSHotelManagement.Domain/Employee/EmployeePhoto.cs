using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 员工照片 (Employee Photo)
    /// </summary>
    [SugarTable("employee_pic", "员工照片 (Employee Photo)")]
    public class EmployeePhoto : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 员工工号 (Employee ID)
        /// </summary>
        [SugarColumn(ColumnName = "employee_number", ColumnDescription = "员工工号 (Employee ID)", Length = 128, IsNullable = false, IsPrimaryKey = true)]
        public string EmployeeId { get; set; }

        /// <summary>
        /// 照片路径 (Photo Path)
        /// </summary>
        [SugarColumn(ColumnName = "pic_url", ColumnDescription = "照片路径 (Photo Path)", Length = 256, IsNullable = true)]
        public string PhotoPath { get; set; }
    }
}
