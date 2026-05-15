using System;

namespace EOM.TSHotelManagement.Domain
{
    [SqlSugar.SugarTable("customer_account", "客户账号表")]
    public class CustomerAccount : SoftDeleteEntity
    {
        /// <summary>
        /// ID (索引ID)
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id", ColumnDescription = "索引ID")]
        public int Id { get; set; }
        /// <summary>
        /// 客户编号 (Customer Number)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "custo_no", IsNullable = false, Length = 128, ColumnDescription = "客户编号 (Customer Number)")]
        public string CustomerNumber { get; set; }
        /// <summary>
        /// 账号 (Account)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "account", ColumnDescription = "账号", Length = 50, IsPrimaryKey = true)]
        public string Account { get; set; }
        /// <summary>
        /// 邮箱 (Email)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "email_address", ColumnDescription = "邮箱 (Email)", Length = 256, IsNullable = true)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// 名称 (Name)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "name", ColumnDescription = "名称", Length = 150, IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// 密码 (Password)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "password", ColumnDescription = "密码", Length = 256)]
        public string Password { get; set; }
        /// <summary>
        /// 状态 (Status)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "status", ColumnDescription = "账号状态")]
        public int Status { get; set; } = 0;
        /// <summary>
        /// 最后一次登录地址 (Last Login IP)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "last_login_ip", ColumnDescription = "最后一次登录地址 (Last Login IP)", Length = 45)]
        public string LastLoginIp { get; set; }
        /// <summary>
        /// 最后一次登录时间 (Last Login Time)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "last_login_time", ColumnDescription = "最后一次登录时间 (Last Login Time)", IsNullable = true)]
        public DateTime? LastLoginTime { get; set; }
    }
}
