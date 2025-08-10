using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastSeasonByIdQuery : ICommand<Result<Season>>
{
    public required string SeasonId { get; set; }
}

public class GetPodcastSeasonByIdQueryHandler(
    IRepository<Season> repository
) : ICommandHandler<GetPodcastSeasonByIdQuery, Result<Season>>
{
    public async Task<Result<Season>> ExecuteAsync(GetPodcastSeasonByIdQuery command, CancellationToken ct)
    {
        var seasonSpec = new SeasonByIdSpec(command.SeasonId);
        var season = await repository.FirstOrDefaultAsync(seasonSpec, ct);

        if (season is not null)
            return Result.Success(season);

        return Result.NotFound();
    }
}