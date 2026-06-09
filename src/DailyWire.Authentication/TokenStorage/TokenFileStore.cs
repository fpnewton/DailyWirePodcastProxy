using System.Text.Json;
using DailyWire.Authentication.Models;

namespace DailyWire.Authentication.TokenStorage;

public class TokenFileStore(string filePath) : ITokenStore
{
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public async Task<AuthenticationTokens?> GetAuthenticationTokensAsync(CancellationToken cancellationToken)
    {
        await _fileLock.WaitAsync(cancellationToken);

        try
        {
            return await ReadTokensAsync(cancellationToken);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task StoreAuthenticationTokensAsync(
        AuthenticationTokens tokens,
        CancellationToken cancellationToken)
    {
        await _fileLock.WaitAsync(cancellationToken);

        try
        {
            await WriteTokensAsync(tokens, cancellationToken);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<bool> TryStoreAuthenticationTokensAsync(
        AuthenticationTokens tokens,
        string expectedRefreshToken,
        CancellationToken cancellationToken)
    {
        await _fileLock.WaitAsync(cancellationToken);

        try
        {
            var currentTokens = await ReadTokensAsync(cancellationToken);

            if (!string.Equals(
                    currentTokens?.RefreshToken,
                    expectedRefreshToken,
                    StringComparison.Ordinal))
            {
                return false;
            }

            await WriteTokensAsync(tokens, cancellationToken);
            return true;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private static async Task<AuthenticationTokens?> DeserializeTokensAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        if (stream.Length == 0)
        {
            return null;
        }

        return await JsonSerializer.DeserializeAsync<AuthenticationTokens>(
            stream,
            cancellationToken: cancellationToken);
    }

    private async Task<AuthenticationTokens?> ReadTokensAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return await DeserializeTokensAsync(stream, cancellationToken);
    }

    private async Task WriteTokensAsync(
        AuthenticationTokens tokens,
        CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(filePath);
        var directory = Path.GetDirectoryName(fullPath)!;
        var temporaryPath = Path.Combine(directory, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await using (var stream = new FileStream(
                             temporaryPath,
                             FileMode.CreateNew,
                             FileAccess.Write,
                             FileShare.None,
                             bufferSize: 4096,
                             FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, tokens, cancellationToken: cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }
}
