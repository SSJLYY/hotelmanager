using System.ComponentModel;

namespace EOM.TSHotelManagement.Common
{
    /// <summary>
    /// 新闻类型
    /// </summary>
    public enum NewsType
    {
        [Description("酒店动态")]
        HotelDynamic = 0,
        [Description("行业新闻")]
        IndustryNews = 1,
        [Description("公司新闻")]
        CompanyNews = 2,
        [Description("其他")]
        Other = 3,
    }
}
