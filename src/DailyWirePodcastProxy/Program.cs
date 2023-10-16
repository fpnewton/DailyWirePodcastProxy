using DailyWireApi.Setup;
using DailyWireAuthentication.Setup;
using DailyWirePodcastProxy.Configuration;
using DailyWirePodcastProxy.Setup;
using PodcastDatabase.Setup;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureHost()
    .ConfigureApi()
    .ConfigureAutoMapper()
    // .ConfigureSimpleInjector(packageAssemblies)
    .ConfigureMediatR()
    .AddPodcastDatabase()
    .ConfigureQuartzServices()
    .AddHostedServices();

builder.Services
    .ConfigureDailyWireApi()
    .ConfigureDailyWireAuthentication()
    .ConfigureDailyWirePodcastProxy()
    .ConfigurePodcastDatabase();

var app = builder.Build()
    .ConfigureHost()
    .ConfigureApi();

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