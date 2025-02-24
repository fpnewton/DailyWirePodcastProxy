using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using DailyWire.Authentication.Models;

namespace DailyWire.Authentication.Handlers;

public class RefreshTokenHandler(OAuthConfiguration oauthConfiguration)
{
    private readonly AuthenticationApiClient _authenticationClient = new(oauthConfiguration.Issuer);
    
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
            Audience = oauthConfiguration.Audience,
            ClientId = oauthConfiguration.ClientId,
            RefreshToken = refreshToken
        }, cancellationToken);
}