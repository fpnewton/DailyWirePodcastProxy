using DailyWire.Authentication.Models;
using DailyWire.Authentication.TokenStorage;

namespace PodcastProxy.Application.Tests.Authentication;

public class TokenFileStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"{nameof(TokenFileStoreTests)}-{Guid.NewGuid():N}");

    [Fact]
    public async Task TryStoreAuthenticationTokensAsync_DoesNotOverwriteNewerTokens()
    {
        Directory.CreateDirectory(_directory);
        var filePath = Path.Combine(_directory, "tokens.json");
        var store = new TokenFileStore(filePath);
        var currentTokens = CreateTokens("current-refresh-token");
        var newerTokens = CreateTokens("newer-refresh-token");
        var staleRefreshResult = CreateTokens("rotated-stale-refresh-token");
        await store.StoreAuthenticationTokensAsync(currentTokens, CancellationToken.None);
        await store.StoreAuthenticationTokensAsync(newerTokens, CancellationToken.None);

        var stored = await store.TryStoreAuthenticationTokensAsync(
            staleRefreshResult,
            currentTokens.RefreshToken!,
            CancellationToken.None);

        Assert.False(stored);
        var persistedTokens = await store.GetAuthenticationTokensAsync(CancellationToken.None);
        Assert.Equal(newerTokens.RefreshToken, persistedTokens?.RefreshToken);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    [Fact]
    public async Task ConcurrentReadsAndWritesAlwaysReturnCompleteTokens()
    {
        Directory.CreateDirectory(_directory);
        var store = new TokenFileStore(Path.Combine(_directory, "tokens.json"));
        await store.StoreAuthenticationTokensAsync(CreateTokens("initial"), CancellationToken.None);

        var writes = Enumerable.Range(0, 50)
            .Select(index => store.StoreAuthenticationTokensAsync(
                CreateTokens($"refresh-{index}"),
                CancellationToken.None));
        var reads = Enumerable.Range(0, 50)
            .Select(async _ =>
            {
                var tokens = await store.GetAuthenticationTokensAsync(CancellationToken.None);
                Assert.NotNull(tokens);
                Assert.NotNull(tokens.AccessToken);
                Assert.NotNull(tokens.RefreshToken);
            });

        await Task.WhenAll(writes.Concat(reads));

        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private static AuthenticationTokens CreateTokens(string refreshToken)
    {
        return new AuthenticationTokens
        {
            AccessToken = $"access-{refreshToken}",
            TokenType = "Bearer",
            ExpiresIn = 3600,
            RefreshToken = refreshToken
        };
    }
}
