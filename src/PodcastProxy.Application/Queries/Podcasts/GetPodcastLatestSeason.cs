using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Application.Queries.Shows;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastLatestSeasonQuery : ICommand<Result<Season>>
{
    public required string PodcastSlug { get; set; }
}

public class GetPodcastLatestSeasonQueryHandler(
    IRepository<Season> repository
) : ICommandHandler<GetPodcastLatestSeasonQuery, Result<Season>>
{
    public async Task<Result<Season>> ExecuteAsync(GetPodcastLatestSeasonQuery command, CancellationToken ct)
    {
        var newSeasons = await new GetShowSeasonsByShowSlugQuery { Slug = command.PodcastSlug }.ExecuteAsync(ct);

        if (!newSeasons.IsSuccess)
            return newSeasons.Map();

        var podcast = await new GetPodcastBySlugQuery { Slug = command.PodcastSlug }.ExecuteAsync(ct);

        if (!podcast.IsSuccess)
            return podcast.Map();

        var existingSeasons = await new GetPodcastSeasonsByPodcastSlugQuery { PodcastSlug = command.PodcastSlug }.ExecuteAsync(ct);

        var seasons = newSeasons.Value
            .OrderByDescending(e => e.Name)
            .Select(e => new Season
            {
                PodcastId = podcast.Value.Id,
                SeasonId = e.Id,
                Slug = e.Slug,
                Name = e.Name
            })
            .ToList();

        if (seasons.Count < 1)
            return Result.NoContent();

        foreach (var season in seasons)
        {
            if (!existingSeasons.IsSuccess || !existingSeasons.Value.Any(s => string.Equals(s.SeasonId, season.SeasonId, StringComparison.Ordinal)))
            {
                await repository.AddAsync(season, ct);
            }
        }

        return Result.Success(seasons.First());
    }
}