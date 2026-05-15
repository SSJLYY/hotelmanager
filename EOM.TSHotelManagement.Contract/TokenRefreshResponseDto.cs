namespace EOM.TSHotelManagement.Contract
{
    public class TokenRefreshResponseDto : BaseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}