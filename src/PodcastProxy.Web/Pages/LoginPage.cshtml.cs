using Auth0.AuthenticationApi.Models;
using DailyWire.Authentication.Handlers;
using DailyWire.Authentication.Models;
using DailyWire.Authentication.Services;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastProxy.Domain.Services;

namespace PodcastProxy.Web.Pages;

public class LoginPage(
    IAuthenticationDetailsProvider authenticationDetailsProvider,
    ITokenService tokenService,
    DeviceCodeLoginHandler handler,
    IConfiguration configuration
) : PageModel
{
        public bool Valid { get; set; }
    
        [BindProperty(SupportsGet = true)]
        public string? Error { get; set; }
    
        public DeviceCodeResponse? Tokens { get; set; }
    
        public string? QrCodeUrl
        {
            get
            {
                if (string.IsNullOrEmpty(Tokens?.VerificationUriComplete))
                {
                    return null;
                }
    
                var uri = $"{Request.Scheme}://{Request.Host}"
                    .AppendPathSegment(configuration["Host:BasePath"])
                    .AppendPathSegment("qr-code")
                    .AppendQueryParam("uri", Tokens.VerificationUriComplete);
    
                if (authenticationDetailsProvider.AccessKeyRequirementEnabled())
                {
                    uri = uri.AppendQueryParam("auth", authenticationDetailsProvider.GetApiAccessKey());
                }
    
                return uri;
            }
        }
    
        public string AuthorizeCallbackUrl
        {
            get
            {
                var uri = $"{Request.Scheme}://{Request.Host}"
                    .AppendPathSegment(configuration["Host:BasePath"])
                    .AppendPathSegment("authorize");
    
                if (authenticationDetailsProvider.AccessKeyRequirementEnabled())
                {
                    uri = uri.AppendQueryParam("auth", authenticationDetailsProvider.GetApiAccessKey());
                }
    
                return uri;
            }
        }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Valid = await tokenService.HasValidAccessToken(cancellationToken);
    
            if (!Valid)
            {
                Tokens = await handler.TryLogin(cancellationToken);
            }
        }
}