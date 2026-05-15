namespace EOM.TSHotelManagement.Infrastructure
{
    public class LskyConfig
    {
        public bool Enabled { get; set; } = true;
        public string BaseAddress { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UploadApi { get; set; }
        public string GetTokenApi { get; set; }
    }
}
