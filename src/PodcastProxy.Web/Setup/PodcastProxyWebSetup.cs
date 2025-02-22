using DailyWire.Authentication.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PodcastProxy.Domain.Services;
using PodcastProxy.Web.Services;

namespace PodcastProxy.Web.Setup;

public static class PodcastProxyWebSetup
{
    public static IServiceCollection ConfigurePodcastProxyWeb(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.TryAddSingleton(ProvideAccountConfiguration);
        services.TryAddSingleton(ProvideOAuthConfiguration);

        services.TryAddSingleton<IAuthenticationDetailsProvider, AuthenticationDetailsProvider>();

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