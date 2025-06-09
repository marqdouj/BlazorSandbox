using Marqdouj.Html.Geolocation;
using Marqdouj.Html.Geolocation.Models;
using Marqdouj.HtmlComponents;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Marqdouj.JSLogger;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();

builder.Services.AddScoped<IGeolocationService, GeolocationService>();
builder.Services.AddScoped<IResizeObserverService, ResizeObserverService>();

/*
For the purpose of this demo, both type of loggers are configured. 
normally you would only configure one type of logger service.
*/
builder.Services.AddLoggerModule(null);
builder.Services.AddLoggerService(null); /*See `App.Razor` for how to add the global script required for this service */

await builder.Build().RunAsync();
