namespace EOM.TSHotelManagement.Infrastructure
{
    public class CsrfTokenConfig
    {
        /// <summary>
        /// CSRF Token 过期时间（分钟）
        /// </summary>
        public int TokenExpirationInMinutes { get; set; } = 120;

        /// <summary>
        /// Token 刷新阈值（分钟）
        /// </summary>
        public int TokenRefreshThresholdInMinutes { get; set; } = 30;

        /// <summary>
        /// Cookie 名称
        /// </summary>
        public string CookieName { get; set; } = "XSRF-TOKEN";

        /// <summary>
        /// Header 名称
        /// </summary>
        public string HeaderName { get; set; } = "X-XSRF-TOKEN";
    }
}