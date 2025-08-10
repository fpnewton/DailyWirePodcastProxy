using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Application.Queries.Shows;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastLatestSeasonQuery : ICommand<Result<Season>>
{
    public required string PodcastId { get; set; }
}

public class GetPodcastLatestSeasonQueryHandler(
    IRepository<Season> repository
) : ICommandHandler<GetPodcastLatestSeasonQuery, Result<Season>>
{
    public async Task<Result<Season>> ExecuteAsync(GetPodcastLatestSeasonQuery command, CancellationToken ct)
    {
        var newSeasons = await new GetShowSeasonsByShowIdQuery { ShowId = command.PodcastId }.ExecuteAsync(ct);

        if (!newSeasons.IsSuccess)
            return newSeasons.Map();

        var spec = new SeasonByPodcastIdSpec(command.PodcastId);
        var existingSeasons = await repository.ListAsync(spec, ct);

        var seasons = newSeasons.Value
            .OrderByDescending(e => e.Slug)
            .Select(e => new Season
            {
                PodcastId = command.PodcastId,
                SeasonId = e.Id,
                Slug = e.Slug,
                Name = e.Name
            })
            .ToList();

        if (seasons.Count < 1)
            return Result.NoContent();

        foreach (var season in seasons)
        {
            if (!existingSeasons.Any(s => string.Equals(s.SeasonId, season.SeasonId, StringComparison.Ordinal)))
            {
                await repository.AddAsync(season, ct);
            }
        }

        return Result.Success(seasons.First());
    }
}