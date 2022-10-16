using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DailyWirePodcastProxy.Authentication;

public class DefaultAuthenticationOptions : AuthenticationSchemeOptions
{
}

public class DefaultAuthenticationHandler : AuthenticationHandler<DefaultAuthenticationOptions>
{
    public DefaultAuthenticationHandler(IOptionsMonitor<DefaultAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(ArraySegment<Claim>.Empty, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        var result = AuthenticateResult.Success(ticket);
        
        return Task.FromResult(result);
    }
}