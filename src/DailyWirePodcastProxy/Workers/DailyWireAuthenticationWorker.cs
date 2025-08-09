using DailyWire.Authentication.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using PodcastProxy.Domain.Services;

namespace DailyWirePodcastProxy.Workers;

public class DailyWireAuthenticationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var lifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStarted.Register((_, ct) => ApplicationStartedCallback(ct).ConfigureAwait(false), null);

        return Task.CompletedTask;
    }

    private async Task ApplicationStartedCallback(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var authDetailsProvider = scope.ServiceProvider.GetRequiredService<IAuthenticationDetailsProvider>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DailyWireAuthenticationWorker>>();
        var hasValidToken = await tokenService.HasValidAccessToken(cancellationToken);

        logger.LogInformation("Token status: {Valid}", hasValidToken ? "Valid" : "Invalid");

        if (!hasValidToken)
        {
            var server = serviceProvider.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();
            var address = addressFeature?.Addresses.FirstOrDefault();
            var apiKeyQueryParam = !string.IsNullOrEmpty(authDetailsProvider.GetApiAccessKey()) && authDetailsProvider.AccessKeyRequirementEnabled() ? $"?auth={authDetailsProvider.GetApiAccessKey()}" : "";
            var authorizePath = "/login" + apiKeyQueryParam;
            var authorizeUrl = !string.IsNullOrEmpty(address) ? address + authorizePath : authorizePath;
            var uri = new Uri(authorizeUrl, UriKind.RelativeOrAbsolute);

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine("Authorization Required!");
            Console.WriteLine();
            Console.WriteLine($"Please visit: {uri}");
            Console.WriteLine("===========================================");
            Console.WriteLine();
        }
    }
}