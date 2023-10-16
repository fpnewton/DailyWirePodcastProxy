using DailyWireAuthentication.Handlers;
using DailyWireAuthentication.Services;
using DailyWireAuthentication.TokenStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DailyWireAuthentication.Setup;

public static class DailyWireAuthenticationSetup
{
    public static IServiceCollection ConfigureDailyWireAuthentication(this IServiceCollection services)
    {
        services.AddHttpClient();
        
        services.TryAddSingleton<ITokenService, TokenService>();
        services.TryAddSingleton(ProvideTokenStore);

        services.TryAddSingleton<PasswordLoginHandler>();
        services.TryAddSingleton<RefreshTokenHandler>();

        return services;
    }

    private static ITokenStore ProvideTokenStore(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>().GetSection("TokenStorage");
        var filePath = configuration["FilePath"];

        return new TokenFileStore(filePath);
    }
}