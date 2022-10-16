namespace DailyWireAuthentication.Models;

public class OAuthConfiguration
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;

    public IEnumerable<string> Scopes => Scope.Split(',').Select(s => s.Trim());
}