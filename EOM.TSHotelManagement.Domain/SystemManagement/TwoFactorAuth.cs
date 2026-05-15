using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    [SugarTable("two_factor_auth", "2FA配置表")]
    [SugarIndex("ux_2fa_employee_pk", nameof(EmployeePk), OrderByType.Asc, true)]
    [SugarIndex("ux_2fa_administrator_pk", nameof(AdministratorPk), OrderByType.Asc, true)]
    [SugarIndex("ux_2fa_customer_account_pk", nameof(CustomerAccountPk), OrderByType.Asc, true)]
    public class TwoFactorAuth : SoftDeleteEntity
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "索引ID")]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "employee_pk", IsNullable = true, ColumnDescription = "员工表主键ID（FK->employee.id）")]
        public int? EmployeePk { get; set; }

        [SugarColumn(ColumnName = "administrator_pk", IsNullable = true, ColumnDescription = "管理员表主键ID（FK->administrator.id）")]
        public int? AdministratorPk { get; set; }

        [SugarColumn(ColumnName = "customer_account_pk", IsNullable = true, ColumnDescription = "客户账号表主键ID（FK->customer_account.id）")]
        public int? CustomerAccountPk { get; set; }

        [SugarColumn(ColumnName = "secret_key", Length = 512, IsNullable = true, ColumnDescription = "2FA密钥（加密存储）")]
        public string? SecretKey { get; set; }

        [SugarColumn(ColumnName = "is_enabled", IsNullable = false, DefaultValue = "0", ColumnDescription = "是否启用2FA（0:否,1:是）")]
        public int IsEnabled { get; set; } = 0;

        [SugarColumn(ColumnName = "enabled_at", IsNullable = true, ColumnDescription = "启用时间")]
        public DateTime? EnabledAt { get; set; }

        [SugarColumn(ColumnName = "last_verified_at", IsNullable = true, ColumnDescription = "最近一次验证时间")]
        public DateTime? LastVerifiedAt { get; set; }

        [SugarColumn(ColumnName = "last_validated_counter", IsNullable = true, ColumnDescription = "last accepted TOTP counter")]
        public long? LastValidatedCounter { get; set; }
    }
}
