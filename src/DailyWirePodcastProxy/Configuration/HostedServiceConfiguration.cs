using DailyWirePodcastProxy.Workers;

namespace DailyWirePodcastProxy.Configuration;

public static class HostedServiceConfiguration
{
    public static WebApplicationBuilder AddHostedServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<ApiAuthenticationWorker>();
        builder.Services.AddHostedService<DailyWireAuthenticationWorker>();
        builder.Services.AddHostedService<DatabaseMigrationWorker>();

        return builder;
    }
}