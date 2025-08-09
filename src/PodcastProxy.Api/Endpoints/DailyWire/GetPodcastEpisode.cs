using Ardalis.Result;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastEpisodeRequest
{
    public required string Slug { get; set; }
}

public class GetPodcastEpisodeEndpoint(
    IConfiguration configuration,
    IDailyWireMiddlewareApi dwApiService
) : Endpoint<GetPodcastEpisodeRequest, PodcastEpisodeResponse>
{
    public override void Configure()
    {
        Get("daily-wire/podcasts/episodes/{Slug}");
        Description(e => e.Produces<DwEpisodeDetails>());
    }

    public override async Task HandleAsync(GetPodcastEpisodeRequest req, CancellationToken ct)
    {
        var episode = await dwApiService.GetEpisode(req.Slug, ct);
        var result = episode.Map(e => MapEpisodeDetails(req, e));

        await this.SendResult(result, ct);
    }

    private PodcastEpisodeResponse MapEpisodeDetails(GetPodcastEpisodeRequest req, DwEpisodeDetails episode)
    {
        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host;

        var audioStreamUrl = new Url($"{scheme}://{host}")
            .AppendPathSegment(configuration["Host:BasePath"])
            .AppendPathSegments("daily-wire", "podcasts", "episodes", req.Slug, "streams", "audio")
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        var videoStreamUrl = new Url($"{scheme}://{host}")
            .AppendPathSegment(configuration["Host:BasePath"])
            .AppendPathSegments("daily-wire", "podcasts", "episodes", req.Slug, "streams", "video")
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        return new()
        {
            Id = episode.Id,
            Slug = episode.Slug,
            EpisodeNumber = episode.EpisodeNumber,
            Title = episode.Title,
            Description = episode.Description,
            ThumbnailUrl = episode.Images.Thumbnail.Portrait,
            PublishedAt = episode.PublishedAt,
            AudioStreamUrl = audioStreamUrl,
            VideoStreamUrl = videoStreamUrl,
            Duration = episode.Duration,
            Status = episode.Status,
            ScheduledAt = episode.ScheduledAt,
            SharingUrl = episode.SharingUrl,
            MediaType = episode.MediaType
        };
    }
}

public class PodcastEpisodeResponse
{
    public required string Id { get; set; }
    public required string Slug { get; set; }
    public string EpisodeNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }
    public string AudioStreamUrl { get; set; } = string.Empty;
    public string VideoStreamUrl { get; set; } = string.Empty;
    public double Duration { get; set; }
    public DwStatus Status { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public string SharingUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
}