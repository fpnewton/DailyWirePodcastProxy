using DailyWire.Authentication.Services;
using Flurl;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PodcastProxy.Domain.Services;

namespace PodcastProxy.Host.Workers;

public class DailyWireAuthenticationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var lifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStarted.Register(() => ApplicationStartedCallback(CancellationToken.None).ConfigureAwait(false));

        return Task.CompletedTask;
    }

    private async Task ApplicationStartedCallback(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var authDetailsProvider = scope.ServiceProvider.GetRequiredService<IAuthenticationDetailsProvider>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DailyWireAuthenticationWorker>>();
        var hasValidToken = await tokenService.HasValidAccessToken(cancellationToken);

        logger.LogInformation("Token status: {Valid}", hasValidToken ? "Valid" : "Invalid");

        if (!hasValidToken)
        {
            var refreshSuccessful = await tokenService.RefreshToken(cancellationToken);

            if (refreshSuccessful)
            {
                return;
            }

            var server = serviceProvider.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();
            var publicHost = configuration["Host:PublicHost"]?.Trim();
            var host = !string.IsNullOrEmpty(publicHost) ? publicHost : addressFeature?.Addresses.FirstOrDefault();
            var basePath = configuration["Host:BasePath"];

            var url = host.AppendPathSegments(basePath, "login");

            if (authDetailsProvider.AccessKeyRequirementEnabled())
            {
                url = url.AppendQueryParam("auth", authDetailsProvider.GetApiAccessKey());
            }

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine("Authorization Required!");
            Console.WriteLine();
            Console.WriteLine($"Please visit: {url}");
            Console.WriteLine("===========================================");
            Console.WriteLine();
        }
    }
}