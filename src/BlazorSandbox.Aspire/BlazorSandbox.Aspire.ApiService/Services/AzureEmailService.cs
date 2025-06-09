using Azure;
using Azure.Communication.Email;
using MimeKit;

namespace BlazorSandbox.Aspire.ApiService.Services
{

    internal class AzureEmailService(ILogger<IEmailService> logger, EmailServiceSettings settings) : IEmailService
    {
        private readonly ILogger<IEmailService> logger = logger;

        public EmailServiceSettings Settings { get; } = settings;

        public async Task SendEmail(MimeMessage message)
        {
            try
            {
                await SendEmail(message.ToEmailMessage());
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
                var message = this.BuildEmailMessage(recipient, subject, body, bodyIsHtml);
                await SendEmail(message);
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
                var message = Settings.BuildErrorMessage(exception);
                await SendEmail(message.ToEmailMessage());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendErrorEmail failed.");
            }
        }

        private async Task SendEmail(EmailMessage message)
        {
            try
            {
                EmailClient emailClient = new(Settings.ConnectionString);
                EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Started, message);

                while (true)
                {
                    await emailSendOperation.UpdateStatusAsync();
                    if (emailSendOperation.HasCompleted)
                    {
                        break;
                    }
                    await Task.Delay(100);
                }

                if (emailSendOperation.HasValue)
                {
                    logger.LogInformation($"Email queued for delivery. Status = {emailSendOperation.Value.Status}");
                }
            }
            catch (RequestFailedException ex)
            {
                logger.LogInformation($"Email send failed with Code = {ex.ErrorCode} and Message = {ex.Message}");
            }
        }
    }
}
