using DailyWireAuthentication.Services;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DailyWirePodcastProxy.Workers;

public class DailyWireAuthenticationWorker : BackgroundService
{
    private readonly ILogger<DailyWireAuthenticationWorker> _logger;
    private readonly Container _container;

    public DailyWireAuthenticationWorker(ILogger<DailyWireAuthenticationWorker> logger, Container container)
    {
        _logger = logger;
        _container = container;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = ThreadScopedLifestyle.BeginScope(_container);
        var tokenService = scope.GetRequiredService<ITokenService>();
        var hasValidToken = await tokenService.HasValidAccessToken(cancellationToken);

        _logger.LogInformation("Token status: {Valid}", hasValidToken ? "Valid" : "Invalid");

        if (!hasValidToken)
        {
            await tokenService.GetAccessToken(cancellationToken);
        }
    }
}