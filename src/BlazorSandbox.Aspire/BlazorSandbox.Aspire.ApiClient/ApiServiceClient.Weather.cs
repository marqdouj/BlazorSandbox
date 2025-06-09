using BlazorSandbox.Aspire.ApiClient.Models;
using System.Net.Http.Json;

namespace BlazorSandbox.Aspire.ApiClient
{
    public interface IWeatherClient
    {
        Task<List<WeatherForecast>> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default);
    }

    internal class WeatherClient(HttpClient httpClient) : IWeatherClient
    {
        public async Task<List<WeatherForecast>> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
        {
            List<WeatherForecast>? forecasts = [];

            await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weather-forecast", cancellationToken))
            {
                if (forecasts.Count >= maxItems)
                {
                    break;
                }
                if (forecast is not null)
                {
                    forecasts.Add(forecast);
                }
            }

            return forecasts;
        }
    }
}