using DailyWire.Api;
using DailyWire.Authentication.Setup;
using DailyWirePodcastProxy.Setup;
using FastEndpoints;
using FastEndpoints.Swagger;
using PodcastProxy.Api;
using PodcastProxy.Application;
using PodcastProxy.Database;
using PodcastProxy.Host.Configuration;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureHost()
    .ConfigureApi()
    .ConfigureAutoMapper()
    .ConfigureMediatR()
    .AddPodcastDatabase()
    .ConfigureQuartzServices()
    .AddHostedServices();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument()
    .ConfigureDailyWireApi()
    .ConfigureDailyWireAuthentication()
    .ConfigureDailyWirePodcastProxy()
    .ConfigurePodcastProxyApi()
    .ConfigurePodcastDatabase()
    .ConfigurePodcastProxy();

var app = builder.Build()
    .ConfigureHost()
    .ConfigureApi();

app.UseFastEndpoints()
    .UseSwaggerGen();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    await app.RunAsync(app.Configuration["Host:Host"]);
}
catch (Exception e)
{
    logger.LogCritical(e, "Application crashed!");

    throw;
}