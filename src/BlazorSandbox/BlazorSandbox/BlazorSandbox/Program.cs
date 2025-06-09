using BlazorSandbox.Components;
using Marqdouj.Html.Geolocation;
using Marqdouj.Html.Geolocation.Models;
using Marqdouj.HtmlComponents;
using Marqdouj.JSLogger;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddFluentUIComponents();

builder.Services.AddScoped<IGeolocationService, GeolocationService>();
builder.Services.AddScoped<IResizeObserverService, ResizeObserverService>();

/*
For the purpose of this demo, both type of loggers are configured. 
normally you would only configure one type of logger service.
*/
builder.AddLoggerModule(null);
builder.AddLoggerService(null); /*See `App.Razor` for how to add the global script required for this service */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorSandbox.Client._Imports).Assembly);

app.Run();
