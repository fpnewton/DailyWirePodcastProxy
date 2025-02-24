using Microsoft.Extensions.DependencyInjection;

namespace PodcastProxy.Api;

public static class PodcastProxyApiSetup
{
    public static IServiceCollection ConfigurePodcastProxyApi(this IServiceCollection services)
    {
        return services;
    }
}