using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PodcastProxy.Domain.Services;

namespace PodcastProxy.Host.Workers;

public class ApiAuthenticationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ApiAuthenticationWorker>>();
        var authDetailsProvider = serviceProvider.GetRequiredService<IAuthenticationDetailsProvider>();
        var apiAccessKey = authDetailsProvider.GetApiAccessKey();

        if (!authDetailsProvider.AccessKeyRequirementEnabled())
        {
            return Task.CompletedTask;
        }

        if (string.IsNullOrEmpty(apiAccessKey))
        {
            apiAccessKey = authDetailsProvider.CreateApiAccessKey();
        }

        logger.LogInformation("API Access Key: {AccessKey}", apiAccessKey);

        return Task.CompletedTask;
    }
}