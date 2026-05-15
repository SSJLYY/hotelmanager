using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    [SugarTable("appointment_notice_type", "任命公告类型 (Appointment AppointmentNotice Type)")]
    public class AppointmentNoticeType : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 公告类型编号 (AppointmentNotice Type Number)
        /// </summary>
        [SugarColumn(ColumnName = "notice_type_number", IsPrimaryKey = true, IsNullable = false, Length = 128, ColumnDescription = "公告类型编号 (AppointmentNotice Type Number)")]
        public string NoticeTypeNumber { get; set; }

        /// <summary>
        /// 公告类型名称 (AppointmentNotice Type Name)
        /// </summary>
        [SugarColumn(ColumnName = "notice_type_name", IsNullable = false, Length = 200, ColumnDescription = "公告类型名称 (AppointmentNotice Type Name)")]
        public string NoticeTypeName { get; set; }
    }
}
