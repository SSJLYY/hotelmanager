using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 导航栏配置表 (Navigation Bar Configuration)
    /// </summary>
    [SugarTable("nav_bar", "导航栏配置表 (Navigation Bar Configuration)", true)]
    public class NavBar : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 导航控件名称 (Navigation Bar Name)
        /// </summary>
        [SugarColumn(
            ColumnName = "nav_name",
            ColumnDescription = "导航控件名称 (Navigation Bar Name)",
            IsNullable = false,
            Length = 50
        )]
        public string NavigationBarName { get; set; }

        /// <summary>
        /// 导航控件排序 (Navigation Bar Order)
        /// </summary>
        [SugarColumn(
            ColumnName = "nav_or",
            ColumnDescription = "导航控件排序 (Navigation Bar Order)",
            IsNullable = false,
            DefaultValue = "0"
        )]
        public int NavigationBarOrder { get; set; }

        /// <summary>
        /// 导航控件图片 (Navigation Bar Image)
        /// </summary>
        [SugarColumn(
            ColumnName = "nav_pic",
            ColumnDescription = "导航控件图片路径 (Navigation Bar Image Path)",
            IsNullable = true,
            Length = 255
        )]
        public string NavigationBarImage { get; set; }

        /// <summary>
        /// 导航控件事件 (Navigation Bar Event)
        /// </summary>
        [SugarColumn(
            ColumnName = "nav_event",
            ColumnDescription = "导航控件事件标识 (Navigation Bar Event Identifier)",
            IsNullable = true,
            Length = 200
        )]
        public string NavigationBarEvent { get; set; }

        /// <summary>
        /// 导航控件左边距 (Navigation Bar Margin Left)
        /// </summary>
        [SugarColumn(
            ColumnName = "margin_left",
            ColumnDescription = "导航控件左边距像素值 (Margin Left in Pixels)",
            IsNullable = false,
            DefaultValue = "0"
        )]
        public int MarginLeft { get; set; }
    }
}
