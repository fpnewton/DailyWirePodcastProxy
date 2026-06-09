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

public class TokenService(ILogger<TokenService> logger, ITokenStore tokenStore, IRefreshTokenHandler refreshTokenHandler)
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
            return tokens.AccessToken;
        }
        
        tokens = await TryRefreshToken(tokens, cancellationToken);

        if (tokens is not null && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            return tokens.AccessToken;
        }
        
        logger.LogError("Could not acquire valid tokens");
        
        throw new AuthenticationException("Authentication failed");
    }

    public async Task<bool> RefreshToken(CancellationToken cancellationToken)
    {
        var tokens = await tokenStore.GetAuthenticationTokensAsync(cancellationToken);

        if (tokens is null)
        {
            logger.LogWarning("Token refresh skipped because no tokens were found in the token store");
            return false;
        }

        return await TryRefreshToken(tokens, cancellationToken) is not null;
    }

    private async Task<AuthenticationTokens?> TryRefreshToken(
        AuthenticationTokens currentTokens,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentTokens.RefreshToken))
        {
            logger.LogWarning("Token refresh skipped because the stored tokens do not include a refresh token");
            return null;
        }

        logger.LogInformation("Attempting to refresh DailyWire authentication tokens");

        try
        {
            var refreshedTokens = await refreshTokenHandler.RefreshTokensAsync(
                currentTokens.RefreshToken,
                cancellationToken);

            if (refreshedTokens is null)
            {
                logger.LogWarning("DailyWire token refresh returned no tokens");
                return null;
            }

            // Auth0 does not always return a new refresh token when rotation is disabled.
            refreshedTokens.RefreshToken ??= currentTokens.RefreshToken;

            if (!ValidateAccessToken(refreshedTokens))
            {
                logger.LogWarning("DailyWire token refresh returned an invalid access token");
                return null;
            }

            await tokenStore.StoreAuthenticationTokensAsync(refreshedTokens, cancellationToken);
            logger.LogInformation("DailyWire authentication tokens refreshed successfully");

            return refreshedTokens;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "DailyWire token refresh failed");
            return null;
        }
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
