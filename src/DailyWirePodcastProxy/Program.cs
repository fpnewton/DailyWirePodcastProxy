using DailyWireApi;
using DailyWireAuthentication;
using DailyWirePodcastProxy;
using DailyWirePodcastProxy.Configuration;
using PodcastDatabase;
using PodcastProxy;

var packages = new List<Type>
{
    typeof(DailyWireApiPackage),
    typeof(DailyWireAuthenticationPackage),
    typeof(DailyWirePodcastProxyPackage),
    typeof(PodcastsDatabasePackage),
    typeof(PodcastProxyPackage)
};

var packageAssemblies = packages.Select(t => t.Assembly).DistinctBy(a => a.GetName()).ToList();

var builder = WebApplication.CreateBuilder(args)
    .ConfigureHost()
    .ConfigureApi()
    .ConfigureAutoMapper(packageAssemblies)
    .ConfigureSimpleInjector(packageAssemblies)
    .ConfigureMediatR(packageAssemblies)
    .AddGraphQLClient()
    .AddPodcastDatabase()
    .ConfigureQuartzServices()
    .AddHostedServices();

var app = builder.Build()
    .ConfigureHost()
    .ConfigureApi()
    .EnableSimpleInjector();

try
{
    await app.RunAsync(app.Configuration["Host:Host"]);
}
catch (Exception e)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    
    logger.LogCritical(e, "Application crashed!");
    
    throw;
}