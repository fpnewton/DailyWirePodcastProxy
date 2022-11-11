using DailyWirePodcastProxy.Extensions;
using DailyWirePodcastProxy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DailyWirePodcastProxy.Authorization;

public class AuthKeyAuthorizationHandler : AuthorizationHandler<AuthKeyAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthKeyAuthorizationHandler> _logger;
    private readonly IOptionsMonitor<AuthOptions> _authOptions;

    public AuthKeyAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ILogger<AuthKeyAuthorizationHandler> logger, IOptionsMonitor<AuthOptions> authOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _authOptions = authOptions;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthKeyAuthorizationRequirement requirement)
    {
        var authOptions = _authOptions.CurrentValue;
        
        _logger.LogDebug("Authorization enabled: {AuthEnabled}", authOptions.Enabled);

        if (!authOptions.Enabled)
        {
            context.Succeed(requirement);
            _logger.LogInformation("Authorization succeeded: Disabled in config");
        }
        else if (_httpContextAccessor.HttpContext is not null)
        {
            var authKey = _httpContextAccessor.HttpContext.Request.Query["auth"].SingleOrDefault();
            var keyMatches = string.Equals(authKey?.Trim(), authOptions.AccessKey.Trim(), StringComparison.Ordinal);

            _logger.LogDebug("Authorization key matches: {KeyMatches}", keyMatches);
            _logger.LogTrace("Expected auth key: {ExpectedAuthKey}\n\tProvided auth key: {ProvidedAuthKey}", authOptions.AccessKey, authKey);

            if (keyMatches)
            {
                context.Succeed(requirement);
                _logger.LogInformation("Authorization succeeded");
            }
            else
            {
                context.Fail();
                _logger.LogWarning("Authorization failed: Auth key parameter did not match\n\tRequest: {Request}", _httpContextAccessor.HttpContext.Request.ToRequestLogLine());
            }
        }
        else
        {
            context.Fail();
            _logger.LogWarning("Authorization failed: No HTTP context!");
        }
        
        return Task.CompletedTask;
    }
}