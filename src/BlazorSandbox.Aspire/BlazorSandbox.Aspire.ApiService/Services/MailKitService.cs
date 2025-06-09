using MailKit.Net.Smtp;
using Marqdouj.Aspire.MailKit.Client;
using MimeKit;

namespace BlazorSandbox.Aspire.ApiService.Services
{
    internal class MailKitService(ILogger<IEmailService> logger, EmailServiceSettings settings, MailKitClientFactory factory) : IEmailService
    {
        private readonly ILogger<IEmailService> logger = logger;


        private readonly MailKitClientFactory factory = factory;

        public EmailServiceSettings Settings { get; } = settings;

        public async Task SendEmail(MimeMessage message)
        {
            try
            {
                ISmtpClient client = await factory.GetSmtpClientAsync();
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendEmail failed.");
            }
        }

        public async Task SendEmail(string recipient, string subject, string body, bool bodyIsHtml)
        {
            try
            {
                ISmtpClient client = await factory.GetSmtpClientAsync();
                var message = this.BuildMimeMessage(recipient, subject, body, bodyIsHtml);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendEmail failed.");
            }
        }

        public async Task SendErrorEmail(Exception exception)
        {
            try
            {
                ISmtpClient client = await factory.GetSmtpClientAsync();
                var message = Settings.BuildErrorMessage(exception);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendErrorEmail failed.");
            }

        }
    }
}
