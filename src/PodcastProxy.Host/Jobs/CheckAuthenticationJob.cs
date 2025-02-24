using DailyWire.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace PodcastProxy.Host.Jobs;

public class CheckAuthenticationJob(IServiceProvider serviceProvider) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var scope = serviceProvider.CreateAsyncScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var hasValidTokens = await tokenService.HasValidAccessToken(context.CancellationToken);
        
        if (!hasValidTokens)
        {
            await tokenService.RefreshToken(context.CancellationToken);
        }
    }
}