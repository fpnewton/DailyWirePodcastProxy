using DailyWireAuthentication.Services;
using Quartz;

namespace DailyWirePodcastProxy.Jobs;

public class CheckAuthenticationJob : IJob
{
    // private readonly Container _container;
    //
    // public CheckAuthenticationJob(Container container)
    // {
    //     _container = container;
    // }

    public async Task Execute(IJobExecutionContext context)
    {
        // var scope = AsyncScopedLifestyle.BeginScope(_container);
        // var tokenService = scope.GetRequiredService<ITokenService>();
        // var hasValidTokens = await tokenService.HasValidAccessToken(context.CancellationToken);
        //
        // if (!hasValidTokens)
        // {
        //     await tokenService.RefreshToken(context.CancellationToken);
        // }
    }
}