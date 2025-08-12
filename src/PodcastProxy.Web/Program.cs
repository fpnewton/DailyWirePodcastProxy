using System.Text.Json.Serialization;
using DailyWire.Api.Middleware;
using DailyWire.Api.Streaming;
using DailyWire.Authentication.Setup;
using FastEndpoints;
using FastEndpoints.Swagger;
using PodcastProxy.Api;
using PodcastProxy.Database;
using PodcastProxy.Host.Configuration;
using PodcastProxy.Web.Setup;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureHost()
    .ConfigureApi()
    .AddPodcastDatabase()
    .ConfigureQuartzServices()
    .AddHostedServices();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument()
    .ConfigureDailyWireMiddlewareApi()
    .ConfigureDailyWireStreamingApi()
    .ConfigureDailyWireAuthentication()
    .ConfigurePodcastProxyWeb()
    .ConfigurePodcastProxyApi()
    .ConfigurePodcastDatabase();

var app = builder.Build()
    .ConfigureHost()
    .ConfigureApi();

app.UseFastEndpoints(config =>
    {
        config.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
    })
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