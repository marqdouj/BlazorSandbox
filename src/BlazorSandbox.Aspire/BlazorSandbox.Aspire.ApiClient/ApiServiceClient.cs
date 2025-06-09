namespace BlazorSandbox.Aspire.ApiClient
{
    public interface IApiServiceClient
    {
        IDevelopmentClient Development { get; }
        INewsletterClient Newsletter { get; }
        IWeatherClient Weather { get; }
    }

    public class ApiServiceClient(HttpClient httpClient) : IApiServiceClient
    {
        public IDevelopmentClient Development { get; } = new DevelopmentClient(httpClient);
        public INewsletterClient Newsletter { get; } = new NewsletterClient(httpClient);
        public IWeatherClient Weather { get; } = new WeatherClient(httpClient);
    }
}
