using System.Text.Json;
using System.Text.Json.Nodes;
using DailyWire.Authentication.Exceptions;
using DailyWire.Authentication.Handlers;
using DailyWire.Authentication.Models;
using DailyWire.Authentication.TokenStorage;
using JWT;
using JWT.Builder;
using Microsoft.Extensions.Logging;

namespace DailyWire.Authentication.Services;

public interface ITokenService
{
    public Task<bool> HasValidAccessToken(CancellationToken cancellationToken);

    public Task<string> GetAccessToken(CancellationToken cancellationToken);

    public Task<bool> RefreshToken(CancellationToken cancellationToken);
}

public class TokenService(ILogger<TokenService> logger, ITokenStore tokenStore, RefreshTokenHandler refreshTokenHandler)
    : ITokenService
{
    public async Task<bool> HasValidAccessToken(CancellationToken cancellationToken)
    {
        var tokens = await tokenStore.GetAuthenticationTokensAsync(cancellationToken);
        var isValid = tokens is not null && ValidateAccessToken(tokens);
        
        return isValid;
    }

    public async Task<string> GetAccessToken(CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting access token");
        
        var tokens = await tokenStore.GetAuthenticationTokensAsync(cancellationToken);
        
        if (tokens is null)
        {
            logger.LogInformation("No access token was found in token store");

            throw new AuthenticationException("No access token was found in token store");
        }
        
        logger.LogDebug("Validating access token");
        
        if (ValidateAccessToken(tokens) && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            logger.LogInformation("Access token is valid");
            
            return tokens.AccessToken;
        }
        
        logger.LogInformation("Refreshing tokens");
        
        tokens = await refreshTokenHandler.RefreshTokensAsync(tokens.RefreshToken, cancellationToken) ??
                 throw new AuthenticationException("Failed to refresh token");
        
        logger.LogDebug("Validating access token");
        
        if (ValidateAccessToken(tokens) && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            logger.LogInformation("Access token is valid");
            
            return tokens.AccessToken;
        }
        
        logger.LogError("Could not acquire valid tokens");
        
        throw new AuthenticationException("Authentication failed");
    }

    public async Task<bool> RefreshToken(CancellationToken cancellationToken)
    {
        var tokens = await tokenStore.GetAuthenticationTokensAsync(cancellationToken);
        
        if (tokens is not null)
        {
            tokens = await refreshTokenHandler.RefreshTokensAsync(tokens.RefreshToken, cancellationToken);
        }
        
        if (tokens is not null && ValidateAccessToken(tokens))
        {
            await tokenStore.StoreAuthenticationTokensAsync(tokens, cancellationToken);

            return true;
        }
        
        return false;
    }

    private static bool ValidateAccessToken(AuthenticationTokens tokens)
    {
        if (string.IsNullOrWhiteSpace(tokens.AccessToken))
        {
            return false;
        }
        
        var parameters = ValidationParameters.None;

        parameters.ValidateIssuedTime = true;
        parameters.ValidateExpirationTime = true;
        parameters.TimeMargin = (int)(tokens.ExpiresIn * 0.75);

        var json = JwtBuilder.Create()
            .WithValidationParameters(parameters)
            .Decode(tokens.AccessToken);

        var token = JsonSerializer.Deserialize<JsonNode>(json);

        if (token is null)
        {
            return false;
        }
        
        var exp = token["exp"]?.GetValue<long>();
        var expiration = DateTimeOffset.FromUnixTimeSeconds(exp ?? 0L);
        var diff = expiration - DateTimeOffset.UtcNow;

        if (diff.TotalSeconds < 0)
        {
            return false;
        }

        return diff.TotalSeconds > tokens.ExpiresIn * 0.75;
    }
}