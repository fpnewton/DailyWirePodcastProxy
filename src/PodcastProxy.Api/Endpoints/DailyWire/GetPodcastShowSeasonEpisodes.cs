using System.Text.Json;
using Ardalis.Result;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using FastEndpoints;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;
using PodcastProxy.Application.Queries.Shows;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastShowSeasonEpisodesRequest
{
    public required string ShowSlug { get; set; }
    public required string SeasonId { get; set; }
    public string? LastPodcastEpisodeId { get; set; }
    public string? LastShowEpisodeId { get; set; }
    public int? ShowOffset { get; set; }
    public int? PodcastOffset { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? OrderBy { get; set; }
    public DwSortOrderDirection? OrderDirection { get; set; }
}

public class GetPodcastShowSeasonEpisodesEndpoint(
    IConfiguration configuration
) : Endpoint<GetPodcastShowSeasonEpisodesRequest, PodcastShowSeasonEpisodesResponse>
{
    public override void Configure()
    {
        Get("daily-wire/podcasts/{ShowSlug}/seasons/{SeasonId}/episodes");
        Description(e => e.Produces<PodcastShowSeasonEpisodesResponse>());
    }

    public override async Task HandleAsync(GetPodcastShowSeasonEpisodesRequest req, CancellationToken ct)
    {
        var episodes = await new GetShowSeasonEpisodesBySeasonIdQuery
        {
            ShowSlug = req.ShowSlug,
            SeasonId = req.SeasonId,
            LastPodcastEpisodeId = req.LastPodcastEpisodeId,
            LastShowEpisodeId = req.LastShowEpisodeId,
            ShowOffset = req.ShowOffset,
            PodcastOffset = req.PodcastOffset,
            PageNumber = req.PageNumber,
            PageSize = req.PageSize,
            OrderBy = req.OrderBy,
            OrderDirection = req.OrderDirection
        }.ExecuteAsync(ct);

        string? nextPageUrl = null;

        if (episodes.IsSuccess && episodes.Value.NextPage is not null)
        {
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host;

            nextPageUrl = new Url($"{scheme}://{host}")
                .AppendPathSegment(configuration["Host:BasePath"])
                .AppendPathSegments("daily-wire", "podcasts", req.ShowSlug, "seasons", req.SeasonId, "episodes")
                .SetQueryParam("auth", configuration["Authentication:AccessKey"])
                .SetQueryParam("lastPodcastEpisodeId", episodes.Value.NextPage.LastPodcastEpisodeId)
                .SetQueryParam("lastShowEpisodeId", episodes.Value.NextPage.LastShowEpisodeId)
                .SetQueryParam("showOffset", req.ShowOffset)
                .SetQueryParam("podcastOffset", req.PodcastOffset)
                .SetQueryParam("pageNumber", req.PageNumber)
                .SetQueryParam("pageSize", req.PageSize)
                .SetQueryParam("orderBy", req.OrderBy)
                .SetQueryParam("orderDirection", JsonSerializer.Serialize(req.OrderDirection))
                .ToString();
        }

        var result = episodes
            .Map(p => new PodcastShowSeasonEpisodesResponse
            {
                Episodes = MapShowEpisodes(p.Episodes).ToList(),
                NextPageUrl = nextPageUrl
            });

        await this.SendResult(result, ct);
    }

    private static IEnumerable<PodcastShowSeasonEpisodeOverview> MapShowEpisodes(IList<DwShowEpisode> episodes)
    {
        foreach (var episode in episodes)
        {
            yield return new PodcastShowSeasonEpisodeOverview
            {
                Id = episode.Id,
                Slug = episode.Slug,
                Title = episode.Title,
                Description = episode.Description,
                Duration = episode.Duration,
                MediaType = episode.MediaType,
                SharingUrl = episode.SharingUrl,
                ThumbnailUrl = episode.Images.Thumbnail.Landscape,
                Status = episode.Status,
                PublishedAt = episode.PublishedAt,
                ScheduledAt = episode.ScheduledAt
            };
        }
    }
}

public class PodcastShowSeasonEpisodesResponse
{
    public IList<PodcastShowSeasonEpisodeOverview> Episodes { get; set; } = [];
    public string? NextPageUrl { get; set; }
}

public class PodcastShowSeasonEpisodeOverview
{
    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Duration { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string SharingUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DwStatus Status { get; set; }
    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
}