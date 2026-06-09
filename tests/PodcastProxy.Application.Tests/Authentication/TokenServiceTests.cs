using System.IdentityModel.Tokens.Jwt;
using DailyWire.Authentication.Handlers;
using DailyWire.Authentication.Models;
using DailyWire.Authentication.Services;
using DailyWire.Authentication.TokenStorage;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace PodcastProxy.Application.Tests.Authentication;

public class TokenServiceTests
{
    [Fact]
    public async Task RefreshToken_PreservesExistingRefreshTokenAndStoresTokens()
    {
        var currentTokens = CreateTokens(DateTime.UtcNow.AddMinutes(-5), "existing-refresh-token");
        var refreshedTokens = CreateTokens(DateTime.UtcNow.AddDays(30), null);
        var tokenStore = Substitute.For<ITokenStore>();
        var refreshHandler = Substitute.For<IRefreshTokenHandler>();
        tokenStore.GetAuthenticationTokensAsync(Arg.Any<CancellationToken>())
            .Returns(currentTokens);
        refreshHandler.RefreshTokensAsync(currentTokens.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(refreshedTokens);
        var service = new TokenService(
            Substitute.For<ILogger<TokenService>>(),
            tokenStore,
            refreshHandler);

        var result = await service.RefreshToken(CancellationToken.None);

        Assert.True(result);
        Assert.Equal(currentTokens.RefreshToken, refreshedTokens.RefreshToken);
        await tokenStore.Received(1)
            .StoreAuthenticationTokensAsync(refreshedTokens, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAccessToken_PersistsRefreshedTokens()
    {
        var currentTokens = CreateTokens(DateTime.UtcNow.AddMinutes(-5), "refresh-token");
        var refreshedTokens = CreateTokens(DateTime.UtcNow.AddDays(30), "rotated-refresh-token");
        var tokenStore = Substitute.For<ITokenStore>();
        var refreshHandler = Substitute.For<IRefreshTokenHandler>();
        tokenStore.GetAuthenticationTokensAsync(Arg.Any<CancellationToken>())
            .Returns(currentTokens);
        refreshHandler.RefreshTokensAsync(currentTokens.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(refreshedTokens);
        var service = new TokenService(
            Substitute.For<ILogger<TokenService>>(),
            tokenStore,
            refreshHandler);

        var accessToken = await service.GetAccessToken(CancellationToken.None);

        Assert.Equal(refreshedTokens.AccessToken, accessToken);
        await tokenStore.Received(1)
            .StoreAuthenticationTokensAsync(refreshedTokens, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshToken_DoesNotCallAuth0WithoutRefreshToken()
    {
        var currentTokens = CreateTokens(DateTime.UtcNow.AddMinutes(-5), null);
        var tokenStore = Substitute.For<ITokenStore>();
        var refreshHandler = Substitute.For<IRefreshTokenHandler>();
        tokenStore.GetAuthenticationTokensAsync(Arg.Any<CancellationToken>())
            .Returns(currentTokens);
        var service = new TokenService(
            Substitute.For<ILogger<TokenService>>(),
            tokenStore,
            refreshHandler);

        var result = await service.RefreshToken(CancellationToken.None);

        Assert.False(result);
        await refreshHandler.DidNotReceive()
            .RefreshTokensAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    private static AuthenticationTokens CreateTokens(DateTime expires, string? refreshToken)
    {
        var accessToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(expires: expires));

        return new AuthenticationTokens
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = 60 * 60,
            RefreshToken = refreshToken
        };
    }
}
