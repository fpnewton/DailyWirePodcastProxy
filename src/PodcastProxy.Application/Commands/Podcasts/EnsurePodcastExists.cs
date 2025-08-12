using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Application.Queries.Podcasts;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Commands.Podcasts;

public class EnsurePodcastExistsCommand : ICommand<Result<Podcast>>
{
    public required string PodcastSlug { get; set; }
}

public class EnsurePodcastExistsCommandHandler : ICommandHandler<EnsurePodcastExistsCommand, Result<Podcast>>
{
    public async Task<Result<Podcast>> ExecuteAsync(EnsurePodcastExistsCommand command, CancellationToken ct)
    {
        var podcast = await new GetPodcastBySlugQuery { Slug = command.PodcastSlug }.ExecuteAsync(ct);

        if (podcast.IsSuccess)
            return podcast;

        podcast = await new FetchPodcastCommand { PodcastSlug = command.PodcastSlug }.ExecuteAsync(ct);

        await new CheckPodcastForNewEpisodesCommand { PodcastSlug = command.PodcastSlug }.ExecuteAsync(ct);

        return podcast;
    }
}