using DailyWireAuthentication.Models;
using DailyWirePodcastProxy.Mappings;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DailyWirePodcastProxy.Setup;

public static class DailyWirePodcastProxySetup
{
    public static IServiceCollection ConfigureDailyWirePodcastProxy(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        services.TryAddSingleton(ProvideAccountConfiguration);
        services.TryAddSingleton(ProvideOAuthConfiguration);
        services.TryAddSingleton<PodcastFeedValueResolver>();
        
        return services;
    }

    private static AccountConfiguration ProvideAccountConfiguration(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var config = new AccountConfiguration();
        
        configuration.GetSection("Account").Bind(config);

        return config;
    }

    private static OAuthConfiguration ProvideOAuthConfiguration(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var config = new OAuthConfiguration();
        
        configuration.GetSection("OAuth").Bind(config);

        return config;
    }
}