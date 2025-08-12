using Ardalis.Result;
using DailyWire.Api.Middleware.Enums;
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
        
            var episode = await new GetPodcastEpisodeByIdQuery { EpisodeId = showEpisode.Id }.ExecuteAsync(ct);
        
            if (episode.IsSuccess)
            {
                episodes.Add(episode.Value);
            }
            else if (episode.Status == ResultStatus.NotFound)
            {
                var ep = new Episode
                {
                    SeasonId = season.Value.SeasonId,
                    EpisodeId = showEpisode.Id,
                    Slug = showEpisode.Slug,
                    Title = showEpisode.Title,
                    Description = showEpisode.Description,
                    Thumbnail = new Uri(showEpisode.Images.Thumbnail.Landscape),
                    Duration = showEpisode.Duration,
                    PublishDate = showEpisode.PublishedAt,
                    ScheduleAt = showEpisode.ScheduledAt
                };
        
                await repository.AddAsync(ep, ct);
            }
        }

        return Result.Success(episodes);
    }
}