using BlazorSandbox.Aspire.ApiClient.Models;

namespace BlazorSandbox.Aspire.ApiService.Endpoints
{
    internal static class Weather
    {
        public static void MapWeatherApi (this WebApplication app)
        {
            string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

            app.MapGet("/weather-forecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");
        }
    }
}
