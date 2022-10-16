using DailyWireAuthentication.Exceptions;
using DailyWireAuthentication.Handlers;
using DailyWireAuthentication.Models;
using DailyWireAuthentication.TokenStorage;
using JWT;
using JWT.Builder;
using Microsoft.Extensions.Logging;

namespace DailyWireAuthentication.Services;

public interface ITokenService
{
    public Task<bool> HasValidAccessToken(CancellationToken cancellationToken);

    public Task<string> GetAccessToken(CancellationToken cancellationToken);

    public Task RefreshToken(CancellationToken cancellationToken);
}

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly ITokenStore _tokenStore;
    private readonly PasswordLoginHandler _passwordLoginHandler;
    private readonly RefreshTokenHandler _refreshTokenHandler;

    public TokenService(ILogger<TokenService> logger, ITokenStore tokenStore, PasswordLoginHandler passwordLoginHandler, RefreshTokenHandler refreshTokenHandler)
    {
        _logger = logger;
        _tokenStore = tokenStore;
        _passwordLoginHandler = passwordLoginHandler;
        _refreshTokenHandler = refreshTokenHandler;
    }

    public async Task<bool> HasValidAccessToken(CancellationToken cancellationToken)
    {
        var tokens = await _tokenStore.GetAuthenticationTokensAsync(cancellationToken);

        if (tokens is null)
        {
            return false;
        }

        return ValidateAccessToken(tokens);
    }

    public async Task<string> GetAccessToken(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting access token");
        
        var tokens = await _tokenStore.GetAuthenticationTokensAsync(cancellationToken);

        if (tokens is null)
        {
            _logger.LogInformation("No access token was found in token store");
            
            tokens = await _passwordLoginHandler.LoginPasswordAsync(cancellationToken) ??
                     throw new AuthenticationException("Failed to login with username/password");
            
            _logger.LogInformation("Storing new tokens in token store");
            
            await _tokenStore.StoreAuthenticationTokensAsync(tokens, cancellationToken);
        }
        
        _logger.LogDebug("Validating access token");

        if (ValidateAccessToken(tokens) && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            _logger.LogInformation("Access token is valid");
            
            return tokens.AccessToken;
        }
        
        _logger.LogInformation("Refreshing tokens");

        tokens = await _refreshTokenHandler.RefreshTokensAsync(tokens.RefreshToken, cancellationToken) ??
                 throw new AuthenticationException("Failed to refresh token");

        _logger.LogDebug("Validating access token");
        
        if (ValidateAccessToken(tokens) && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            _logger.LogInformation("Access token is valid");
            
            return tokens.AccessToken;
        }
        
        _logger.LogError("Could not acquire valid tokens");

        throw new AuthenticationException("Authentication failed");
    }

    public async Task RefreshToken(CancellationToken cancellationToken)
    {
        var tokens = await _tokenStore.GetAuthenticationTokensAsync(cancellationToken);

        if (tokens is not null)
        {
            tokens = await _refreshTokenHandler.RefreshTokensAsync(tokens.RefreshToken, cancellationToken);
        }
        else
        {
            tokens = await _passwordLoginHandler.LoginPasswordAsync(cancellationToken);
        }
        
        if (tokens is not null && ValidateAccessToken(tokens))
        {
            await _tokenStore.StoreAuthenticationTokensAsync(tokens, cancellationToken);
        }
    }

    private bool ValidateAccessToken(AuthenticationTokens tokens)
    {
        var parameters = ValidationParameters.None;

        parameters.ValidateIssuedTime = true;
        parameters.ValidateExpirationTime = true;
        parameters.TimeMargin = (int)(tokens.ExpiresIn * 0.75);

        var json = JwtBuilder.Create()
            .WithValidationParameters(parameters)
            .Decode(tokens.AccessToken);

        return !string.IsNullOrEmpty(json);
    }
}