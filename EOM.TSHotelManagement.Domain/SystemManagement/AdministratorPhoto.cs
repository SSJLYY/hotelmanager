using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 管理员头像
    /// </summary>
    [SugarTable("administrator_pic", "管理员头像")]
    public class AdministratorPhoto : SoftDeleteEntity
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号")]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "admin_number", ColumnDescription = "管理员编号", Length = 128, IsNullable = false, IsPrimaryKey = true)]
        public string AdminNumber { get; set; }

        [SugarColumn(ColumnName = "pic_url", ColumnDescription = "头像地址", Length = 256, IsNullable = true)]
        public string PhotoPath { get; set; }
    }
}
