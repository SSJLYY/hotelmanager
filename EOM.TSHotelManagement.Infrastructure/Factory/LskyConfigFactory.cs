using Microsoft.Extensions.Configuration;

namespace EOM.TSHotelManagement.Infrastructure
{
    public class LskyConfigFactory
    {
        private readonly IConfiguration _configuration;

        public LskyConfigFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LskyConfig GetLskyConfig()
        {
            var lskySection = _configuration.GetSection("Lsky");
            var lskyConfig = new LskyConfig
            {
                Enabled = lskySection.GetValue<bool?>("Enabled") ?? false,
                BaseAddress = lskySection.GetValue<string>("BaseAddress") ?? string.Empty,
                Email = lskySection.GetValue<string>("Email") ?? string.Empty,
                Password = lskySection.GetValue<string>("Password") ?? string.Empty,
                UploadApi = lskySection.GetValue<string>("UploadApi") ?? string.Empty,
                GetTokenApi = lskySection.GetValue<string>("GetTokenApi") ?? string.Empty
            };
            return lskyConfig;
        }
    }
}
