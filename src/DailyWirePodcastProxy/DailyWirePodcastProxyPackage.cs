using DailyWireAuthentication.Models;
using DailyWirePodcastProxy.Mappings;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace DailyWirePodcastProxy;

public class DailyWirePodcastProxyPackage : IPackage
{
    public void RegisterServices(Container container)
    {
        container.Register(() => ProvideAccountConfiguration(container));
        container.Register(() => ProvideOAuthConfiguration(container));
        
        container.Register<PodcastFeedValueResolver>();
    }

    private AccountConfiguration ProvideAccountConfiguration(Container container)
    {
        var configuration = container.GetRequiredService<IConfiguration>();
        var config = new AccountConfiguration();
        
        configuration.GetSection("Account").Bind(config);

        return config;
    }

    private OAuthConfiguration ProvideOAuthConfiguration(Container container)
    {
        var configuration = container.GetRequiredService<IConfiguration>();
        var config = new OAuthConfiguration();
        
        configuration.GetSection("OAuth").Bind(config);

        return config;
    }
}