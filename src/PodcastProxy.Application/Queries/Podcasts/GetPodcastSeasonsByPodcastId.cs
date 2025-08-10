using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastSeasonsByPodcastIdQuery : ICommand<Result<List<Season>>>
{
    public required string PodcastId { get; set; }
}

public class GetPodcastSeasonsByPodcastIdQueryHandler(
    IRepository<Season> repository
) : ICommandHandler<GetPodcastSeasonsByPodcastIdQuery, Result<List<Season>>>
{
    public async Task<Result<List<Season>>> ExecuteAsync(GetPodcastSeasonsByPodcastIdQuery command, CancellationToken ct)
    {
        var spec = new SeasonByPodcastIdSpec(command.PodcastId);
        var seasons = await repository.ListAsync(spec, ct);

        return Result.Success(seasons);
    }
}