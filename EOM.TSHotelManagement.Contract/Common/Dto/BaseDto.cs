namespace EOM.TSHotelManagement.Contract
{
    public class BaseDto
    {
        public int? Id { get; set; }
        /// <summary>
        /// Access Token
        /// </summary>
        public string UserToken { get; set; }
        /// <summary>
        /// Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
