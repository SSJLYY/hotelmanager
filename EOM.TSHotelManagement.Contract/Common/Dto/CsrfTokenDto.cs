namespace EOM.TSHotelManagement.Contract
{
    public class CsrfTokenDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool NeedsRefresh { get; set; }
    }
}
