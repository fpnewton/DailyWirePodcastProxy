using DailyWireAuthentication.Handlers;
using DailyWireAuthentication.Services;
using DailyWireAuthentication.TokenStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace DailyWireAuthentication;

public class DailyWireAuthenticationPackage : IPackage
{
    public void RegisterServices(Container container)
    {
        container.Register(() => ProvideTokenStore(container));
        
        container.Register<ITokenService, TokenService>();
        
        container.Register<PasswordLoginHandler>();
        container.Register<RefreshTokenHandler>();
    }

    private ITokenStore ProvideTokenStore(Container container)
    {
        var configuration = container.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection("TokenStorage");
        var filePath = section["FilePath"];

        return new TokenFileStore(filePath);
    }
}