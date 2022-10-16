using Microsoft.AspNetCore.Authorization;

namespace DailyWirePodcastProxy.Authorization;

public class AuthKeyAuthorizationHandler : AuthorizationHandler<AuthKeyAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    private string ConfigAccessKey => _configuration["Authentication:AccessKey"];

    public AuthKeyAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthKeyAuthorizationRequirement requirement)
    {
        if (_httpContextAccessor.HttpContext is not null)
        {
            var authKey = _httpContextAccessor.HttpContext.Request.Query["auth"].Select(s => s.Trim()).SingleOrDefault();

            if (string.Equals(authKey, ConfigAccessKey))
            {
                context.Succeed(requirement);
            }
        }

        if (!context.HasSucceeded)
        {
            context.Fail();
        }
        
        return Task.CompletedTask;
    }
}