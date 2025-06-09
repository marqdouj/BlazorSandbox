using BlazorSandbox.Aspire.ApiService.Services;

namespace BlazorSandbox.Aspire.ApiService.Endpoints
{
    internal static class Newsletter
    {
        private const string emailDefault = "email.test@dummy.com";

        /// <summary>
        /// If email is empty, return the default email.(i.e. OpenAPI UI Testing)
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static string GetEmail(string email) => string.IsNullOrWhiteSpace(email) ? emailDefault : email;

        public static void MapNewsletterApi(this WebApplication app)
        {
            app.MapGet("/newsletter/is-subscribed", (string email) =>
            {
                return email == emailDefault;
            })
            .WithName("Is-Subscribed");

            app.MapPost("/newsletter/subscribe", async (IEmailService service, string email) =>
            {
                email = GetEmail(email);
                var message = service.BuildNewsletterMessage(email, true);
                await service.SendEmail(message);
            })
            .WithName("Subscribe");

            app.MapPost("/newsletter/unsubscribe", async (IEmailService service, string email) =>
            {
                email = GetEmail(email);
                var message = service.BuildNewsletterMessage(email, false);
                await service.SendEmail(message);
            })
            .WithName("Unsubscribe");
        }
    }
}
