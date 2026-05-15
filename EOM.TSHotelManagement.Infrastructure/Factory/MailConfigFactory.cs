using Microsoft.Extensions.Configuration;

namespace EOM.TSHotelManagement.Infrastructure
{
    public class MailConfigFactory
    {
        private readonly IConfiguration _configuration;

        public MailConfigFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MailConfig GetMailConfig()
        {
            var mailConfig = new MailConfig
            {
                Host = _configuration.GetSection("Mail").GetValue<string>("Host") ?? string.Empty,
                Port = _configuration.GetSection("Mail").GetValue<int?>("Port") ?? 587,
                UserName = _configuration.GetSection("Mail").GetValue<string>("UserName") ?? string.Empty,
                Password = _configuration.GetSection("Mail").GetValue<string>("Password") ?? string.Empty,
                EnableSsl = _configuration.GetSection("Mail").GetValue<bool?>("EnableSsl") ?? false,
                DisplayName = _configuration.GetSection("Mail").GetValue<string>("DisplayName") ?? string.Empty,
                Enabled = _configuration.GetSection("Mail").GetValue<bool?>("Enabled") ?? false,
            };
            return mailConfig;
        }
    }
}
