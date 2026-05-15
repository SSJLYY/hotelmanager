using EOM.TSHotelManagement.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;

namespace EOM.TSHotelManagement.Common
{
    public class MailHelper
    {
        private readonly MailConfigFactory mailConfigFactory;
        private readonly ILogger<MailHelper> logger;

        public MailHelper(MailConfigFactory mailConfigFactory, ILogger<MailHelper> logger)
        {
            this.mailConfigFactory = mailConfigFactory;
            this.logger = logger;
        }

        public bool SendMail(
            List<string> toEmails,
            string subject,
            string body,
            List<string> ccEmails = null,
            List<string> bccEmails = null,
            List<string> attachments = null,
            bool isBodyHtml = true)
        {
            var mailConfig = mailConfigFactory.GetMailConfig();

            if (!mailConfig.Enabled)
            {
                logger.LogError("Email sending is disabled in the configuration.");
                return false;
            }

            if (!IsMailConfigValid())
            {
                logger.LogError("Invalid mail configuration.");
                return false;
            }

            if (!toEmails.Any())
            {
                logger.LogError("No recipient email addresses provided.");
                return false;
            }

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(mailConfig.DisplayName, mailConfig.UserName));

            AddRecipients(message.To, toEmails);

            AddRecipients(message.Cc, ccEmails);

            AddRecipients(message.Bcc, bccEmails);

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = isBodyHtml ? body : null,
                TextBody = isBodyHtml ? null : body
            };

            AddAttachments(bodyBuilder, attachments);

            message.Subject = subject;
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                client.Connect(
                    mailConfig.Host,
                    mailConfig.Port,
                    GetSecureSocketOptions()
                );

                client.Authenticate(mailConfig.UserName, mailConfig.Password);

                client.Send(message);

                return true;
            }
            catch (AuthenticationException ex)
            {
                throw new ApplicationException(LocalizationHelper.GetLocalizedString($"Email verification failed: {ex.Message}", $"邮件认证失败: {ex.Message}"), ex);
            }
            catch (SmtpCommandException ex)
            {
                throw new ApplicationException(LocalizationHelper.GetLocalizedString($"SMTP command error ({ex.StatusCode}): {ex.Message}", $"SMTP命令错误 ({ex.StatusCode}): {ex.Message}"), ex);
            }
            catch (SmtpProtocolException ex)
            {
                throw new ApplicationException(LocalizationHelper.GetLocalizedString($"SMTP protocol error: {ex.Message}", $"SMTP协议错误: {ex.Message}"), ex);
            }
            finally
            {
                client.Disconnect(true);
            }
        }


        public async Task<bool> CheckServiceStatusAsync()
        {
            var mailConfig = mailConfigFactory.GetMailConfig();
            var host = mailConfig.Host;
            var port = mailConfig.Port;
            var enabled = mailConfig.Enabled;

            try
            {
                if (!enabled)
                {
                    logger.LogWarning("邮件服务未启用, 跳过检查");
                    return false;
                }

                if (port == 0)
                {
                    logger.LogError("邮件服务配置信息缺失");
                    return false;
                }

                using var client = new TcpClient();
                await client.ConnectAsync(host, port);
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                string response = await reader.ReadLineAsync();
                if (response?.StartsWith("220") == true)
                {
                    using var writer = new StreamWriter(stream);
                    await writer.WriteLineAsync("QUIT");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "邮件服务连接异常");
                return false;
            }
        }

        #region Private Methods

        private bool IsMailConfigValid()
        {
            try
            {
                var config = mailConfigFactory.GetMailConfig();

                if (config == null)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(config.Host))
                {
                    return false;
                }

                if (config.Port <= 0 || config.Port > 65535)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(config.UserName))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(config.Password))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(config.DisplayName))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AddRecipients(InternetAddressList list, List<string> emails)
        {
            emails?.Where(email => !string.IsNullOrWhiteSpace(email))
                   .ToList()
                   .ForEach(email => list.Add(MailboxAddress.Parse(email)));
        }

        private void AddAttachments(BodyBuilder builder, List<string> attachments)
        {
            attachments?.Where(File.Exists)
                       .ToList()
                       .ForEach(filePath =>
                       {
                           using var stream = File.OpenRead(filePath);
                           builder.Attachments.Add(
                               Path.GetFileName(filePath),
                               stream,
                               MimeKit.ContentType.Parse(MediaTypeNames.Application.Octet)
                           );
                       });
        }

        private SecureSocketOptions GetSecureSocketOptions()
        {
            var mailConfig = mailConfigFactory.GetMailConfig();
            return mailConfig.EnableSsl switch
            {
                true when mailConfig.Port == 465 => SecureSocketOptions.SslOnConnect,
                true when mailConfig.Port == 587 => SecureSocketOptions.StartTls,
                true => SecureSocketOptions.Auto,
                false => SecureSocketOptions.None
            };
        }

        #endregion
    }
}