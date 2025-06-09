using Microsoft.AspNetCore.WebUtilities;

namespace BlazorSandbox.Aspire.ApiClient
{
    public interface INewsletterClient
    {
        Task SubscribeToNewsletterAsync(string email, CancellationToken cancellationToken = default);
        Task UnSubscribeToNewsletterAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> IsSubscribedToNewsletterAsync(string email, CancellationToken cancellationToken = default);
    }

    internal class NewsletterClient(HttpClient httpClient) : INewsletterClient
    {
        public async Task<bool> IsSubscribedToNewsletterAsync(string email, CancellationToken cancellationToken = default)
        {
            var values = new Dictionary<string, string?>
            {
                { nameof(email), email }
            };

            var q = QueryHelpers.AddQueryString("/newsletter/is-subscribed", values);
            var answer = await httpClient.GetStringAsync(q, cancellationToken);
            return bool.Parse(answer);
        }

        public async Task SubscribeToNewsletterAsync(string email, CancellationToken cancellationToken = default)
        {
            var values = new Dictionary<string, string?>
            {
                { nameof(email), email }
            };

            var q = QueryHelpers.AddQueryString("/newsletter/subscribe", values);
            await httpClient.PostAsync(q, null, cancellationToken);
        }

        public async Task UnSubscribeToNewsletterAsync(string email, CancellationToken cancellationToken = default)
        {
            var values = new Dictionary<string, string?>
            {
                { nameof(email), email }
            };

            var q = QueryHelpers.AddQueryString("/newsletter/unsubscribe", values);
            await httpClient.PostAsync(q, null, cancellationToken);
        }
    }
}
