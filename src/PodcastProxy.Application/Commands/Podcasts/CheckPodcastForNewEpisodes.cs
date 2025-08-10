using FastEndpoints;
using PodcastProxy.Application.Queries.Podcasts;

namespace PodcastProxy.Application.Commands.Podcasts;

public class CheckPodcastForNewEpisodesCommand : ICommand
{
    public required string PodcastId { get; set; }
}

public class CheckPodcastForNewEpisodesCommandHandler : ICommandHandler<CheckPodcastForNewEpisodesCommand>
{
    public async Task ExecuteAsync(CheckPodcastForNewEpisodesCommand command, CancellationToken ct)
    {
        var season = await new GetPodcastLatestSeasonQuery { PodcastId = command.PodcastId }.ExecuteAsync(ct);

        if (!season.IsSuccess)
            return;

        await new FetchPodcastsLatestEpisodesCommand { SeasonId = season.Value.SeasonId }.ExecuteAsync(ct);
    }
}