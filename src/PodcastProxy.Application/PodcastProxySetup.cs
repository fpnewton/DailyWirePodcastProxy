using Microsoft.Extensions.DependencyInjection;
using PodcastProxy.Application.Commands;

namespace PodcastProxy.Application;

public static class PodcastProxySetup
{
    public static IServiceCollection ConfigurePodcastProxy(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            config.AddBehavior<FetchPodcastSeasonsEnsurePodcastExistsPipeline>();
        });
        
        return services;
    }
}