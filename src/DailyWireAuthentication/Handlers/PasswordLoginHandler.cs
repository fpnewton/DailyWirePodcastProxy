using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using DailyWireAuthentication.Models;
using Flurl;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace DailyWireAuthentication.Handlers;

public class PasswordLoginHandler
{
    private readonly AccountConfiguration _accountConfiguration;
    private readonly OAuthConfiguration _oauthConfiguration;
    private readonly AuthenticationApiClient _authenticationClient;

    private Uri AuthorizationUrl => _authenticationClient.BuildAuthorizationUrl()
        .WithAudience(_oauthConfiguration.Audience)
        .WithClient(_oauthConfiguration.ClientId)
        .WithRedirectUrl(_oauthConfiguration.RedirectUrl)
        .WithScopes(_oauthConfiguration.Scopes.ToArray())
        .WithResponseType(AuthorizationResponseType.Code)
        .Build();

    public PasswordLoginHandler(AccountConfiguration accountConfiguration, OAuthConfiguration oauthConfiguration)
    {
        _accountConfiguration = accountConfiguration;
        _oauthConfiguration = oauthConfiguration;
        _authenticationClient = new AuthenticationApiClient(_oauthConfiguration.Issuer);
    }

    public async Task<AuthenticationTokens?> LoginPasswordAsync(CancellationToken cancellationToken)
    {
        var authorizationCode = await GetAuthorizationCode();

        if (string.IsNullOrEmpty(authorizationCode))
        {
            return null;
        }

        var tokens = await GetTokens(authorizationCode, cancellationToken);

        if (tokens is null)
        {
            return null;
        }

        return new AuthenticationTokens
        {
            TokenType = tokens.TokenType,
            AccessToken = tokens.AccessToken,
            IdToken = tokens.IdToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresIn = tokens.ExpiresIn
        };
    }

    private async Task<string?> GetAuthorizationCode()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync(AuthorizationUrl.ToString(), WaitUntilNavigation.Networkidle2);

        await page.WaitForNetworkIdleAsync();
        await page.WaitForSelectorAsync("input[name=email]", new WaitForSelectorOptions { Visible = true, Timeout = 3 * 60 * 1000 });
        await page.TypeAsync("input[name=email]", _accountConfiguration.Username, new TypeOptions { Delay = 50 });

        await page.WaitForNetworkIdleAsync();
        await page.WaitForSelectorAsync("input[name=password]", new WaitForSelectorOptions { Visible = true, Timeout = 3 * 60 * 1000 });
        await page.TypeAsync("input[name=password]", _accountConfiguration.Password, new TypeOptions { Delay = 50 });

        await page.Keyboard.PressAsync("Enter");
        await page.WaitForNetworkIdleAsync();

        return new Url(page.Url).QueryParams.FirstOrDefault("code")?.ToString();
    }

    private async Task<AccessTokenResponse?> GetTokens(string authorizationCode, CancellationToken cancellationToken) =>
        await _authenticationClient.GetTokenAsync(new AuthorizationCodeTokenRequest
        {
            ClientId = _oauthConfiguration.ClientId,
            Code = authorizationCode,
            RedirectUri = _oauthConfiguration.RedirectUrl
        }, cancellationToken);
}