using FastEndpoints;
using PodcastProxy.Application.Queries.Podcasts;

namespace PodcastProxy.Application.Commands.Podcasts;

public class CheckAllPodcastsForNewEpisodesCommand : ICommand;

public class CheckAllPodcastsForNewEpisodesCommandHandler : ICommandHandler<CheckAllPodcastsForNewEpisodesCommand>
{
    public async Task ExecuteAsync(CheckAllPodcastsForNewEpisodesCommand command, CancellationToken ct)
    {
        var podcasts = await new GetPodcastsQuery().ExecuteAsync(ct);

        if (!podcasts.IsSuccess)
            return;

        foreach (var podcast in podcasts.Value)
        {
            await new CheckPodcastForNewEpisodesCommand { PodcastId = podcast.Id }.ExecuteAsync(ct);
        }
    }
}