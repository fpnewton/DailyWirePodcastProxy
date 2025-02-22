using DailyWire.Authentication.Handlers;
using DailyWire.Authentication.Services;
using DailyWire.Authentication.TokenStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DailyWire.Authentication.Setup;

public static class DailyWireAuthenticationSetup
{
    public static IServiceCollection ConfigureDailyWireAuthentication(this IServiceCollection services)
    {
        services.TryAddSingleton<ITokenService, TokenService>();
        services.TryAddSingleton(ProvideTokenStore);

        services.TryAddSingleton<DeviceCodeLoginHandler>();
        services.TryAddSingleton<RefreshTokenHandler>();

        return services;
    }

    private static ITokenStore ProvideTokenStore(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>().GetSection("TokenStorage");
        var filePath = configuration["FilePath"];

        if (string.IsNullOrEmpty(filePath))
        {
            throw new NullReferenceException("App configuration section 'TokenStorage' is missing 'FilePath' property.");
        }

        return new TokenFileStore(filePath);
    }
}