using System.Text;
using System.Text.Json;
using DailyWireAuthentication.Models;

namespace DailyWireAuthentication.TokenStorage;

public class TokenFileStore : ITokenStore
{
    private readonly string _filePath;

    public TokenFileStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<AuthenticationTokens?> GetAuthenticationTokensAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(_filePath, Encoding.UTF8, cancellationToken);

        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<AuthenticationTokens>(json);
    }

    public async Task StoreAuthenticationTokensAsync(AuthenticationTokens? tokens, CancellationToken cancellationToken)
    {
        if (tokens is null)
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
            
            return;
        }

        var json = JsonSerializer.Serialize(tokens);

        await File.WriteAllTextAsync(_filePath, json, Encoding.UTF8, cancellationToken);
    }
}