using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    [SugarTable("two_factor_recovery_code", "2FA recovery codes")]
    [SugarIndex("idx_2fa_recovery_auth", nameof(TwoFactorAuthPk), OrderByType.Asc)]
    [SugarIndex("idx_2fa_recovery_used", nameof(IsUsed), OrderByType.Asc)]
    public class TwoFactorRecoveryCode : SoftDeleteEntity
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "Primary key")]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "two_factor_auth_pk", IsNullable = false, ColumnDescription = "FK->two_factor_auth.id")]
        public int TwoFactorAuthPk { get; set; }

        [SugarColumn(ColumnName = "code_salt", Length = 64, IsNullable = false, ColumnDescription = "Recovery code salt")]
        public string CodeSalt { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "code_hash", Length = 128, IsNullable = false, ColumnDescription = "Recovery code hash")]
        public string CodeHash { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "is_used", IsNullable = false, DefaultValue = "0", ColumnDescription = "Whether used")]
        public int IsUsed { get; set; } = 0;

        [SugarColumn(ColumnName = "used_at", IsNullable = true, ColumnDescription = "Used time")]
        public DateTime? UsedAt { get; set; }
    }
}
