using Ardalis.Result;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using FastEndpoints;
using PodcastProxy.Application.Queries.Podcasts;
using PodcastProxy.Application.Queries.Shows;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Commands.Podcasts;

public class FetchPodcastsLatestEpisodesCommand : ICommand<Result<List<Episode>>>
{
    public required string SeasonId { get; set; }
}

public class FetchPodcastsLatestEpisodesCommandHandler(
    IRepository<Episode> repository
) : ICommandHandler<FetchPodcastsLatestEpisodesCommand, Result<List<Episode>>>
{
    public async Task<Result<List<Episode>>> ExecuteAsync(FetchPodcastsLatestEpisodesCommand command, CancellationToken ct)
    {
        var season = await new GetPodcastSeasonByIdQuery { SeasonId = command.SeasonId }.ExecuteAsync(ct);

        if (!season.IsSuccess)
            return season.Map();

        var podcast = await new GetPodcastByIdQuery { PodcastId = season.Value.PodcastId }.ExecuteAsync(ct);

        if (!podcast.IsSuccess)
            return podcast.Map();
        
        var newEpisodes = await new GetLatestShowEpisodesQuery { ShowSlug = podcast.Value.Slug }.ExecuteAsync(ct);

        if (!newEpisodes.IsSuccess)
            return newEpisodes.Map();
        
        var episodes = new List<Episode>();
        
        foreach (var showEpisode in newEpisodes.Value)
        {
            if (showEpisode.Status != DwStatus.Published)
                continue;
        
            var episodeResult = await new GetPodcastEpisodeByIdQuery { EpisodeId = showEpisode.Id }.ExecuteAsync(ct);
        
            if (episodeResult.IsSuccess)
            {
                var episode = episodeResult.Value;

                MapShowEpisode(episode, showEpisode, season.Value.SeasonId);

                await repository.UpdateAsync(episode, ct);
                episodes.Add(episode);
            }
            else if (episodeResult.Status == ResultStatus.NotFound)
            {
                var episode = new Episode { EpisodeId = showEpisode.Id };

                MapShowEpisode(episode, showEpisode, season.Value.SeasonId);
        
                await repository.AddAsync(episode, ct);
                episodes.Add(episode);
            }
        }

        return Result.Success(episodes);
    }

    private static void MapShowEpisode(Episode episode, DwShowEpisode showEpisode, string seasonId)
    {
        episode.SeasonId = seasonId;
        episode.Slug = showEpisode.Slug;
        episode.Title = showEpisode.Title;
        episode.Description = showEpisode.Description;
        episode.Thumbnail = new Uri(showEpisode.Images.Thumbnail.Landscape);
        episode.Duration = showEpisode.Duration;
        episode.PublishDate = showEpisode.PublishedAt;
        episode.ScheduleAt = showEpisode.ScheduledAt;
    }
}
