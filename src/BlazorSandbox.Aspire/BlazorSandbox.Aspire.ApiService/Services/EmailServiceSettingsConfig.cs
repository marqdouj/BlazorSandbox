namespace BlazorSandbox.Aspire.ApiService.Services
{
    internal class EmailServiceSettingsConfig
    {
        public string? ConnectionString { get; set; }
        public List<string> ErrorRecipients { get; set; } = [];
        public string? Sender { get; set; }
    }
}
