using DailyWireAuthentication.Models;

namespace DailyWireAuthentication.TokenStorage;

public interface ITokenStore
{
    public Task<AuthenticationTokens?> GetAuthenticationTokensAsync(CancellationToken cancellationToken);

    public Task StoreAuthenticationTokensAsync(AuthenticationTokens token, CancellationToken cancellationToken);
}