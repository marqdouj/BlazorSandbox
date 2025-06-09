using BlazorSandbox.Aspire.ApiService.Endpoints;
using BlazorSandbox.Aspire.ApiService.Services;
using Marqdouj.CLRCommon;
using Microsoft.AspNetCore.Diagnostics;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.ConfigureEmailService();

var app = builder.Build();

app.MapDefaultEndpoints();

//Configure email service for exception handling
app.UseExceptionHandler(exApp => exApp.Run(async context =>
{
    if (context.Response.HasStarted)
        return;

    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var exMsg = exception?.ToMessage();


    if (exception != null)
    {
        ILogger<IEmailService>? logger = null;
        exMsg = $"{exception.Source}: {exMsg}";

        try
        {
            try
            {
                var factory = exApp.ApplicationServices.GetService<ILoggerFactory>();
                logger = factory?.CreateLogger<IEmailService>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create logger: {ex}");
            }

            using var scope = exApp.ApplicationServices.CreateScope();
            var emailService = scope.ServiceProvider.GetService<IEmailService>();
            if (emailService != null)
                await emailService.SendErrorEmail(exception);
            else
            {
                logger?.LogError(exception, "ExceptionHandler - Failed to get email service.");
                Console.WriteLine("ExceptionHandler - Failed to get email service.");
            }
        }
        catch (Exception)
        {
            logger?.LogError(exception, "ExceptionHandler - SendErrorEmail failed.");
            Console.WriteLine("ExceptionHandler - SendErrorEmail failed.");
        }
    }

    var pds = context.RequestServices.GetService<IProblemDetailsService>();
    var ok = false;

    if (pds != null)
    {
        if (string.IsNullOrWhiteSpace(exMsg))
        {
            ok = await pds.TryWriteAsync(new() { HttpContext = context });
        }
        else
        {
            ok = await pds.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails =
                {
                    Title = exMsg,
                }
            });
        }
    }

    if (ok is false)
    {
        // Fallback behavior
        await context.Response.WriteAsync($"Fallback: An error has occurred. {exMsg}");
    }
}));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.DefaultFonts = false; // Disable default fonts to avoid download unnecessary fonts
        options.Servers = []; //Required in Aspire
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.MapDevelopment();
app.MapNewsletterApi();
app.MapWeatherApi();

app.Run();
