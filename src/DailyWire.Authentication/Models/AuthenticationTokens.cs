namespace DailyWire.Authentication.Models;

public class AuthenticationTokens
{
    public string? AccessToken { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string? IdToken { get; set; }
    public int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
}