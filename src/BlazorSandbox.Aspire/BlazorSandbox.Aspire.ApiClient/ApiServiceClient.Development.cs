using Microsoft.AspNetCore.WebUtilities;

namespace BlazorSandbox.Aspire.ApiClient
{
    public interface IDevelopmentClient
    {
        Task ThrowException(string? message, CancellationToken cancellationToken = default);
    }

    internal class DevelopmentClient(HttpClient httpClient) : IDevelopmentClient
    {
        public async Task ThrowException(string? message, CancellationToken cancellationToken = default)
        {
            var values = new Dictionary<string, string?>
            {
                { nameof(message), message }
            };

            var q = QueryHelpers.AddQueryString("/development/throw-exception", values);
            await httpClient.GetAsync(q, cancellationToken);
        }
    }
}
