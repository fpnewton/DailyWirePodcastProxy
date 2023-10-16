using DailyWireAuthentication.Services;

namespace DailyWirePodcastProxy.Workers;

public class DailyWireAuthenticationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DailyWireAuthenticationWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DailyWireAuthenticationWorker>>();
        var hasValidToken = await tokenService.HasValidAccessToken(cancellationToken);
        
        logger.LogInformation("Token status: {Valid}", hasValidToken ? "Valid" : "Invalid");
        
        if (!hasValidToken)
        {
            await tokenService.GetAccessToken(cancellationToken);
        }
    }
}