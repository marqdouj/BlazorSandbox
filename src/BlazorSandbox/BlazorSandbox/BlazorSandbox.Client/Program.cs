using Marqdouj.Html.Geolocation;
using Marqdouj.Html.Geolocation.Models;
using Marqdouj.HtmlComponents;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();

builder.Services.AddScoped<IGeolocationService, GeolocationService>();
builder.Services.AddScoped<IResizeObserverService, ResizeObserverService>();

await builder.Build().RunAsync();
