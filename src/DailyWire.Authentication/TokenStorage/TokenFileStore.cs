using System.Text;
using System.Text.Json;
using DailyWire.Authentication.Models;

namespace DailyWire.Authentication.TokenStorage;

public class TokenFileStore(string filePath) : ITokenStore
{
    public async Task<AuthenticationTokens?> GetAuthenticationTokensAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken);

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
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            return;
        }

        var json = JsonSerializer.Serialize(tokens);

        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8, cancellationToken);
    }
}