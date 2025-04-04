using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodcastProxy.Domain.Models;
using PodcastProxy.Host.Extensions;

namespace PodcastProxy.Host.Authorization;

public class AuthKeyAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    ILogger<AuthKeyAuthorizationHandler> logger,
    IOptionsMonitor<AuthOptions> options
) : AuthorizationHandler<AuthKeyAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthKeyAuthorizationRequirement requirement)
    {
        var authOptions = options.CurrentValue;

        logger.LogDebug("Authorization enabled: {AuthEnabled}", authOptions.Enabled);

        if (!authOptions.Enabled)
        {
            context.Succeed(requirement);
            logger.LogInformation("Authorization succeeded: Disabled in config");
        }
        else if (httpContextAccessor.HttpContext is not null)
        {
            var authKey = httpContextAccessor.HttpContext.Request.Query["auth"].SingleOrDefault();
            var keyMatches = string.Equals(authKey?.Trim(), authOptions.AccessKey.Trim(), StringComparison.Ordinal);

            var sanitizedAuthKey = authKey?.Replace(Environment.NewLine, "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
            
            logger.LogDebug("Authorization key matches: {KeyMatches}", keyMatches);
            logger.LogTrace("Expected auth key: {ExpectedAuthKey}\n\tProvided auth key: {ProvidedAuthKey}", authOptions.AccessKey, sanitizedAuthKey);

            if (keyMatches)
            {
                context.Succeed(requirement);
                logger.LogInformation("Authorization succeeded");
            }
            else
            {
                context.Fail();
                logger.LogWarning("Authorization failed: Auth key parameter '{ProvidedAuthKey}' did not match '{ExpectedAuthKey}'\n\tRequest: {Request}",
                    httpContextAccessor.HttpContext.Request.ToRequestLogLine(), sanitizedAuthKey, authOptions.AccessKey);
            }
        }
        else
        {
            context.Fail();
            logger.LogWarning("Authorization failed: No HTTP context!");
        }

        return Task.CompletedTask;
    }
}