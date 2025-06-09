using MimeKit;

namespace BlazorSandbox.Aspire.ApiService.Services
{
    internal record EmailServiceSettings(
        MailboxAddress Sender,
        List<MailboxAddress> ErrorRecipients,
        string? ConnectionString);
}
