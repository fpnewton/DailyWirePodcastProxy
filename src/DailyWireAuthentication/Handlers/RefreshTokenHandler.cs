using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using DailyWireAuthentication.Models;

namespace DailyWireAuthentication.Handlers;

public class RefreshTokenHandler
{
    private readonly OAuthConfiguration _oauthConfiguration;
    private readonly AuthenticationApiClient _authenticationClient;

    public RefreshTokenHandler(OAuthConfiguration oauthConfiguration)
    {
        _oauthConfiguration = oauthConfiguration;
        _authenticationClient = new AuthenticationApiClient(_oauthConfiguration.Issuer);
    }

    public async Task<AuthenticationTokens?> RefreshTokensAsync(string? refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }

        var tokens = await GetTokens(refreshToken, cancellationToken);

        if (tokens is null)
        {
            return null;
        }
        
        return new AuthenticationTokens
        {
            TokenType = tokens.TokenType,
            AccessToken = tokens.AccessToken,
            IdToken = tokens.IdToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresIn = tokens.ExpiresIn
        };
    }

    private async Task<AccessTokenResponse?> GetTokens(string refreshToken, CancellationToken cancellationToken) =>
        await _authenticationClient.GetTokenAsync(new RefreshTokenRequest
        {
            Audience = _oauthConfiguration.Audience,
            ClientId = _oauthConfiguration.ClientId,
            RefreshToken = refreshToken
        }, cancellationToken);
}