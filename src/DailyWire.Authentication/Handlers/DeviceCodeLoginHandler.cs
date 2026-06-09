using Ardalis.Result;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using DailyWire.Authentication.Models;
using Microsoft.Extensions.Logging;

namespace DailyWire.Authentication.Handlers;

public class DeviceCodeLoginHandler(
    OAuthConfiguration oauthConfiguration,
    ILogger<DeviceCodeLoginHandler> logger)
{
    private readonly AuthenticationApiClient _authenticationClient = new(oauthConfiguration.Issuer);

    public async Task<DeviceCodeResponse?> TryLogin(CancellationToken cancellationToken)
    {
        var request = new DeviceCodeRequest
        {
            Audience = oauthConfiguration.Audience,
            ClientId = oauthConfiguration.ClientId,
            Scope = oauthConfiguration.Scope
        };

        var result = await _authenticationClient.StartDeviceFlowAsync(request, cancellationToken);

        if (result is null)
        {
            throw new Exception("Unable to start device code flow login.");
        }
        
        return result;
    }

    public async Task<Result<AuthenticationTokens>> GetUserToken(string deviceCode, CancellationToken cancellationToken)
    {
        var request = new DeviceCodeTokenRequest
        {
            ClientId = oauthConfiguration.ClientId,
            DeviceCode = deviceCode
        };

        try
        {
            var result = await _authenticationClient.GetTokenAsync(request, cancellationToken);

            var tokens = new AuthenticationTokens
            {
                AccessToken = result.AccessToken,
                TokenType = result.TokenType,
                IdToken = result.IdToken,
                ExpiresIn = result.ExpiresIn,
                RefreshToken = result.RefreshToken
            };

            if (string.IsNullOrWhiteSpace(tokens.RefreshToken))
            {
                logger.LogWarning(
                    "DailyWire authentication succeeded, but Auth0 did not issue a refresh token. " +
                    "Automatic renewal will not be possible when the access token expires");
            }
            
            return Result<AuthenticationTokens>.Success(tokens);
        }
        catch (Exception e)
        {
            return Result<AuthenticationTokens>.Error(e.Message);
        }
    }
}
