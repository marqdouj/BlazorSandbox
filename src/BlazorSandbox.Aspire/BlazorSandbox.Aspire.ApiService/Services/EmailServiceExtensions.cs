using Azure.Communication.Email;
using Marqdouj.CLRCommon;
using MimeKit;
using System.Text;

namespace BlazorSandbox.Aspire.ApiService.Services
{
    internal static class EmailServiceExtensions
    {
        public static void ConfigureEmailService(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                var useAzureEmailInDev = false; // change to true to test azure email in IDE

                if (useAzureEmailInDev)
                {
                    builder.AddAzureMailService();
                }
                else
                {
                    builder.AddMailKitService();
                }
            }
            else
            {
                builder.AddAzureMailService();
            }
        }

        public static EmailMessage ToEmailMessage(this MimeMessage message)
        {
            var sender = message.Sender.Address;
            var to = message.To.Cast<MailboxAddress>().Select(e => new EmailAddress(e.Address, e.Name));
            var cc = message.Cc.Cast<MailboxAddress>().Select(e => new EmailAddress(e.Address, e.Name));
            var bcc = message.Bcc.Cast<MailboxAddress>().Select(e => new EmailAddress(e.Address, e.Name));
            var recipients = new EmailRecipients(to, cc, bcc);
            var content = new EmailContent(message.Subject)
            {
                Html = message.HtmlBody,
                PlainText = message.TextBody
            };

            return new EmailMessage(sender, recipients, content);
        }

        public static MimeMessage BuildMimeMessage(this IEmailService service, string recipient, string subject, string body, bool bodyIsHtml)
        {
            var bodyType = bodyIsHtml ? "html" : "plain";

            var message = new MimeMessage
            {
                Sender = service.Settings.Sender
            };
            //message.From.Add(service.Settings.Sender);
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = subject;
            message.Body = new TextPart(bodyType) { Text = body };

            return message;
        }

        public static EmailMessage BuildEmailMessage(this IEmailService service, string recipient, string subject, string body, bool bodyIsHtml)
        {
            var sender = service.Settings.Sender.Address;
            var content = new EmailContent(subject);

            if (bodyIsHtml)
                content.Html = body;
            else
                content.PlainText = body;

            return new EmailMessage(sender, recipient, content);
        }

        public static MimeMessage BuildNewsletterMessage(this IEmailService service, string recipient, bool subscribe)
        {
            var subject = subscribe ? "Welcome to our newsletter!" : "You are unsubscribed from our newsletter!";
            var body = subscribe ? "Thank you for subscribing to our newsletter!" : "Sorry to see you go. We hope you will come back soon!";

            var message = new MimeMessage
            {
                Sender = service.Settings.Sender
            };
            //message.From.Add(service.Settings.Sender);
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            return message;
        }

        public static MimeMessage BuildErrorMessage(this EmailServiceSettings settings, Exception exception)
        {
            var message = new MimeMessage
            {
                Sender = settings.Sender
            };
            //message.From.Add(settings.Sender);
            message.To.AddRange(settings.ErrorRecipients);
            message.Subject = $"DEMO Exception - {exception.Source}";

            var body = new StringBuilder();
            body.AppendLine("The following exception has occurred:");
            body.AppendLine();
            body.AppendLine($"{exception.ToMessage()}");
            message.Body = new TextPart("plain") { Text = body.ToString() };

            return message;
        }

        public static WebApplicationBuilder AddMailKitService(this WebApplicationBuilder builder) 
        {
            var settings = builder.BuildSettings("MailKitEmail");

            builder.AddMailKitClient("maildev");
            builder.Services.AddSingleton(settings);
            builder.Services.AddScoped<IEmailService, MailKitService>();
            return builder;
        }

        public static WebApplicationBuilder AddAzureMailService(this WebApplicationBuilder builder)
        {
            var settings = builder.BuildSettings("AzureEmail");
            builder.Services.AddSingleton(settings);
            builder.Services.AddScoped<IEmailService, AzureEmailService>();

            return builder;
        }

        private static EmailServiceSettings BuildSettings(this WebApplicationBuilder builder, string section)
        {
            var config = new EmailServiceSettingsConfig();
            var jsonSettings = builder.Configuration.GetRequiredSection(section);
            jsonSettings.Bind(config);

            var settings = new EmailServiceSettings(
                new MailboxAddress("DEMO", config.Sender),
                [.. config.ErrorRecipients!.Select(e => new MailboxAddress("", e))],
                config.ConnectionString);

            return settings;
        }
    }
}
