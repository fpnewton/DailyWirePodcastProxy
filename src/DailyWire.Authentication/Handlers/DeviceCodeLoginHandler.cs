using Ardalis.Result;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using DailyWire.Authentication.Models;

namespace DailyWire.Authentication.Handlers;

public class DeviceCodeLoginHandler(OAuthConfiguration oauthConfiguration)
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
            ClientId = "FCgw3nA6cxkcXLVseAQvCSVBrymwvfpE",
            DeviceCode = deviceCode
        };

        try
        {
            var result = await _authenticationClient.GetTokenAsync(request, cancellationToken);

            if (result is null)
            {
                return Result<AuthenticationTokens>.Error("Did not receive user tokens.");
            }

            var tokens = new AuthenticationTokens
            {
                AccessToken = result.AccessToken,
                TokenType = result.TokenType,
                IdToken = result.IdToken,
                ExpiresIn = result.ExpiresIn,
                RefreshToken = result.RefreshToken
            };
            
            return Result<AuthenticationTokens>.Success(tokens);
        }
        catch (Exception e)
        {
            return Result<AuthenticationTokens>.Error(e.Message);
        }
    }
}