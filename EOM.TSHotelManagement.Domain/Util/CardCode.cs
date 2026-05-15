using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 卡片代码 (Card Codes)
    /// </summary>
    [SugarTable("card_code", "卡片代码 (Card Codes)", true)]
    public class CardCode : BaseEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 省份 (Province)
        /// </summary>
        [SugarColumn(ColumnName = "province", IsNullable = false, Length = 100, ColumnDescription = "省份 (Province)")]
        public string Province { get; set; }

        /// <summary>
        /// 城市 (City)
        /// </summary>
        [SugarColumn(ColumnName = "city", IsNullable = true, Length = 100, ColumnDescription = "城市 (City)")]
        public string City { get; set; }

        /// <summary>
        /// 地区 (District)
        /// </summary>
        [SugarColumn(ColumnName = "district", IsNullable = true, Length = 100, ColumnDescription = "地区 (District)")]
        public string District { get; set; }

        /// <summary>
        /// 地区识别码 (Area Code)
        /// </summary>
        [SugarColumn(ColumnName = "district_code", IsNullable = false, Length = 50, ColumnDescription = "地区识别码 (Area Code)")]
        public string AreaCode { get; set; }
    }
}
