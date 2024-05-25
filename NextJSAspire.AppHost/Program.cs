using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//builder.AddProject<Projects.WebApi>("webapi");

var api = builder.AddProject<Projects.WebApi>("webapi");

var frontend = builder.AddNpmApp(name: "frontend", workingDirectory: "../frontend", scriptName: "dev")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(api);

if (builder.Environment.IsDevelopment() && builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https")
{
    // Disable TLS certificate validation in development, see + https://github.com/dotnet/aspire/issues/3324 for more details.
    frontend.WithEnvironment("NODE_TLS_REJECT_UNAUTHORIZED", "0");
}

builder.AddProject<Projects.ReverseProxy>("reverseproxy")
    .WithReference(frontend)
    .WithReference(api);

builder.Build().Run();
