using DailyWire.Authentication.Models;

namespace DailyWire.Authentication.TokenStorage;

public interface ITokenStore
{
    public Task<AuthenticationTokens?> GetAuthenticationTokensAsync(CancellationToken cancellationToken);

    public Task StoreAuthenticationTokensAsync(AuthenticationTokens token, CancellationToken cancellationToken);
}