using System;
using System.Globalization;

namespace EOM.TSHotelManagement.Common
{
    public static class LocalizationHelper
    {
        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="englishText">英文文本</param>
        /// <param name="chineseText">中文文本</param>
        /// <returns>根据当前文化返回相应的文本</returns>
        public static string GetLocalizedString(string englishText, string chineseText)
        {
            var culture = CultureInfo.CurrentCulture.Name;
            return culture.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ? chineseText : englishText;
        }

        /// <summary>
        /// 设置当前文化
        /// </summary>
        /// <param name="culture">文化名称</param>
        public static void SetCulture(string culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }
    }
}
