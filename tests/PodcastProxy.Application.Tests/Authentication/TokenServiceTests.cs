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
        refreshHandler.RefreshTokensAsync(currentTokens.RefreshToken!, Arg.Any<CancellationToken>())
            .Returns(refreshedTokens);
        tokenStore.TryStoreAuthenticationTokensAsync(
                refreshedTokens,
                currentTokens.RefreshToken!,
                Arg.Any<CancellationToken>())
            .Returns(true);
        var service = new TokenService(
            Substitute.For<ILogger<TokenService>>(),
            tokenStore,
            refreshHandler);

        var result = await service.RefreshToken(CancellationToken.None);

        Assert.True(result);
        Assert.Equal(currentTokens.RefreshToken, refreshedTokens.RefreshToken);
        await tokenStore.Received(1)
            .TryStoreAuthenticationTokensAsync(
                refreshedTokens,
                currentTokens.RefreshToken!,
                Arg.Any<CancellationToken>());
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
        refreshHandler.RefreshTokensAsync(currentTokens.RefreshToken!, Arg.Any<CancellationToken>())
            .Returns(refreshedTokens);
        tokenStore.TryStoreAuthenticationTokensAsync(
                refreshedTokens,
                currentTokens.RefreshToken!,
                Arg.Any<CancellationToken>())
            .Returns(true);
        var service = new TokenService(
            Substitute.For<ILogger<TokenService>>(),
            tokenStore,
            refreshHandler);

        var accessToken = await service.GetAccessToken(CancellationToken.None);

        Assert.Equal(refreshedTokens.AccessToken, accessToken);
        await tokenStore.Received(1)
            .TryStoreAuthenticationTokensAsync(
                refreshedTokens,
                currentTokens.RefreshToken!,
                Arg.Any<CancellationToken>());
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

    [Fact]
    public async Task RefreshToken_ConcurrentCallsOnlyRotateCurrentRefreshTokenOnce()
    {
        var currentTokens = CreateTokens(DateTime.UtcNow.AddMinutes(-5), "refresh-token");
        var refreshedTokens = CreateTokens(DateTime.UtcNow.AddDays(30), "rotated-refresh-token");
        var tokenStore = new InMemoryTokenStore(currentTokens);
        var refreshHandler = new DelayedRefreshTokenHandler(refreshedTokens);
        var service = new TokenService(
            Substitute.For<ILogger<TokenService>>(),
            tokenStore,
            refreshHandler);

        var results = await Task.WhenAll(
            service.RefreshToken(CancellationToken.None),
            service.RefreshToken(CancellationToken.None));

        Assert.All(results, Assert.True);
        Assert.Equal(1, refreshHandler.CallCount);
        Assert.Same(refreshedTokens, await tokenStore.GetAuthenticationTokensAsync(CancellationToken.None));
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

    private sealed class InMemoryTokenStore(AuthenticationTokens? tokens) : ITokenStore
    {
        private readonly SemaphoreSlim _lock = new(1, 1);
        private AuthenticationTokens? _tokens = tokens;

        public async Task<AuthenticationTokens?> GetAuthenticationTokensAsync(
            CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);

            try
            {
                return _tokens;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task StoreAuthenticationTokensAsync(
            AuthenticationTokens token,
            CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);

            try
            {
                _tokens = token;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<bool> TryStoreAuthenticationTokensAsync(
            AuthenticationTokens token,
            string expectedRefreshToken,
            CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);

            try
            {
                if (!string.Equals(
                        _tokens?.RefreshToken,
                        expectedRefreshToken,
                        StringComparison.Ordinal))
                {
                    return false;
                }

                _tokens = token;
                return true;
            }
            finally
            {
                _lock.Release();
            }
        }
    }

    private sealed class DelayedRefreshTokenHandler(AuthenticationTokens refreshedTokens)
        : IRefreshTokenHandler
    {
        private int _callCount;

        public int CallCount => _callCount;

        public async Task<AuthenticationTokens?> RefreshTokensAsync(
            string? refreshToken,
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _callCount);
            await Task.Delay(50, cancellationToken);
            return refreshedTokens;
        }
    }
}
