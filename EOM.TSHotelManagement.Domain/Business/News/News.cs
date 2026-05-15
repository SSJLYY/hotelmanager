using System;

namespace EOM.TSHotelManagement.Domain
{
    [SqlSugar.SugarTable("news", "新闻动态")]
    public class News : SoftDeleteEntity
    {
        [SqlSugar.SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "索引ID")]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_id", Length = 128, IsNullable = false, ColumnDescription = "新闻编号")]
        public string NewId { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_title", Length = 256, IsNullable = false, ColumnDescription = "新闻标题")]
        public string NewsTitle { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_content", IsNullable = false, ColumnDescription = "新闻内容")]
        public string NewsContent { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_type", Length = 64, IsNullable = false, ColumnDescription = "新闻类型")]
        public string NewsType { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_link", Length = 200, IsNullable = false, ColumnDescription = "新闻链接")]
        public string NewsLink { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_date", IsNullable = false, ColumnDescription = "新闻日期")]
        public DateTime NewsDate { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_status", Length = 64, IsNullable = false, ColumnDescription = "新闻状态")]
        public string NewsStatus { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "news_image", Length = 200, IsNullable = true, ColumnDescription = "新闻图片")]
        public string NewsImage { get; set; }
    }
}
