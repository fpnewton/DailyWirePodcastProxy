using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastEpisodesBySeasonIdQuery : ICommand<Result<List<Episode>>>
{
    public required string SeasonId { get; set; }
}

public class GetPodcastEpisodesBySeasonIdQueryHandler(
    IRepository<Episode> repository
) : ICommandHandler<GetPodcastEpisodesBySeasonIdQuery, Result<List<Episode>>>
{
    public async Task<Result<List<Episode>>> ExecuteAsync(GetPodcastEpisodesBySeasonIdQuery command, CancellationToken ct)
    {
        var spec = new EpisodeBySeasonIdSpec(command.SeasonId);
        var episodes = await repository.ListAsync(spec, ct);

        return Result.Success(episodes);
    }
}