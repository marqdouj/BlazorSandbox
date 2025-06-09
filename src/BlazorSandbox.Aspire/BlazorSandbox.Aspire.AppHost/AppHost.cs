var builder = DistributedApplication.CreateBuilder(args);

// https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/secure-communication-between-integrations
// For this demo the values are placed in appsettings.json.
// Normally, you would use a secret manager to store these values
var mailDevUsername = builder.AddParameter("maildev-username");
var mailDevPassword = builder.AddParameter("maildev-password");
var maildev = builder.AddMailDev(
    name: "maildev",
    userName: mailDevUsername,
    password: mailDevPassword);

var apiService = builder.AddProject<Projects.BlazorSandbox_Aspire_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(maildev);

builder.AddProject<Projects.BlazorSandbox_Aspire_WebApp>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(maildev);

builder.Build().Run();
