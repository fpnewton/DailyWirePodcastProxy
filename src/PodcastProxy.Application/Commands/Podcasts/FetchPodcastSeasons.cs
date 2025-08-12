using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using FastEndpoints;
using PodcastProxy.Application.Queries.Shows;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Commands.Podcasts;

public class FetchPodcastSeasonsCommand : ICommand<Result<List<Season>>>
{
    public string PodcastSlug { get; set; } = string.Empty;
}

public class FetchPodcastSeasonsCommandHandler(
    IRepository<Season> repository
) : ICommandHandler<FetchPodcastSeasonsCommand, Result<List<Season>>>
{
    public async Task<Result<List<Season>>> ExecuteAsync(FetchPodcastSeasonsCommand command, CancellationToken ct)
    {
        var podcast = await new EnsurePodcastExistsCommand { PodcastSlug = command.PodcastSlug }.ExecuteAsync(ct);

        if (!podcast.IsSuccess)
            return podcast.Map();

        var spec = new SeasonByPodcastIdSpec(podcast.Value.Id);
        var seasons = await repository.ListAsync(spec, ct);

        var result = await FetchPodcastSeasons(podcast.Value.Id, command.PodcastSlug, ct);

        if (!result.IsSuccess)
            return result.Map();

        var missing = result.Value.Except(seasons, Season.DefaultComparer).ToList();
        var existing = result.Value.Intersect(seasons, Season.DefaultComparer).ToList();

        if (missing.Count > 0)
        {
            await repository.AddRangeAsync(missing, ct);
        }

        if (existing.Count > 0)
        {
            await repository.UpdateRangeAsync(existing, ct);
        }

        return result;
    }

    private async Task<Result<List<Season>>> FetchPodcastSeasons(string podcastId, string podcastSlug, CancellationToken ct)
    {
        var seasons = await new GetShowSeasonsByShowSlugQuery { Slug = podcastSlug }.ExecuteAsync(ct);

        return seasons.Map(s => MapEntitiesToSeasons(podcastId, s));
    }

    private List<Season> MapEntitiesToSeasons(string podcastId, IList<DwEntity> entities) => entities
        .Select(e => new Season
        {
            PodcastId = podcastId,
            SeasonId = e.Id,
            Slug = e.Slug,
            Name = e.Name
        })
        .ToList();
}