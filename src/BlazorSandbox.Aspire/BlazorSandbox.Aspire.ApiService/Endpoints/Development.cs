namespace BlazorSandbox.Aspire.ApiService.Endpoints
{
    internal static class Development
    {
        public static void MapDevelopment(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
                return;

            app.MapGet("/development/throw-exception", (string? message) =>
            {
                message ??= "This is a test for 'unhandled' exception in an API method.";
                Console.WriteLine($"/development/throw-exception [{DateTime.Now}]:{message}");

                //The IDE will stop here when the API gets called; just continue execution.
                throw new Exception(message);
            })
            .WithName("Throw-Exception");
        }
    }
}
