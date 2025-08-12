using FastEndpoints;
using Microsoft.Extensions.Logging;
using PodcastProxy.Application.Commands.Podcasts;
using Quartz;

namespace PodcastProxy.Host.Jobs;

public class CheckForNewEpisodesJob(ILogger<CheckForNewEpisodesJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Checking for new episodes.");
        
        await new CheckAllPodcastsForNewEpisodesCommand().ExecuteAsync(context.CancellationToken);
        
        logger.LogInformation("Finished checking for new episodes.");
    }
}