using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PodcastProxy.Host.Authentication;

public class DefaultAuthenticationOptions : AuthenticationSchemeOptions
{
}

public class DefaultAuthenticationHandler(IOptionsMonitor<DefaultAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<DefaultAuthenticationOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(ArraySegment<Claim>.Empty, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        var result = AuthenticateResult.Success(ticket);
        
        return Task.FromResult(result);
    }
}