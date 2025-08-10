using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastSeasonsByPodcastSlugQuery : ICommand<Result<List<Season>>>
{
    public required string PodcastSlug { get; set; }
}

public class GetPodcastSeasonsByPodcastSlugQueryHandler : ICommandHandler<GetPodcastSeasonsByPodcastSlugQuery, Result<List<Season>>>
{
    public async Task<Result<List<Season>>> ExecuteAsync(GetPodcastSeasonsByPodcastSlugQuery command, CancellationToken ct)
    {
        var podcast = await new GetPodcastBySlugQuery { Slug = command.PodcastSlug }.ExecuteAsync(ct);

        if (!podcast.IsSuccess)
            return podcast.Map();

        return await new GetPodcastSeasonsByPodcastIdQuery { PodcastId = podcast.Value.Id }.ExecuteAsync(ct);
    }
}