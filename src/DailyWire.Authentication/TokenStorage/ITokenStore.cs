using DailyWire.Authentication.Models;

namespace DailyWire.Authentication.TokenStorage;

public interface ITokenStore
{
    public Task<AuthenticationTokens?> GetAuthenticationTokensAsync(CancellationToken cancellationToken);

    public Task StoreAuthenticationTokensAsync(AuthenticationTokens token, CancellationToken cancellationToken);

    public Task<bool> TryStoreAuthenticationTokensAsync(
        AuthenticationTokens token,
        string expectedRefreshToken,
        CancellationToken cancellationToken);
}
