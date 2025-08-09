using Ardalis.Result;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.ComponentItems;
using DailyWire.Api.Middleware.Models.Components;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;

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
    IConfiguration configuration,
    IDailyWireMiddlewareApi dwApiService
    ) : Endpoint<GetPodcastShowSeasonEpisodesRequest, PodcastShowSeasonEpisodesResponse>
{
    public override void Configure()
    {
        Get("daily-wire/podcasts/{ShowSlug}/seasons/{SeasonId}/episodes");
        Description(e => e.Produces<PodcastShowSeasonEpisodesResponse>());
    }

    public override async Task HandleAsync(GetPodcastShowSeasonEpisodesRequest req, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
        {
            AddError("Failed to get user info.");

            await SendErrorsAsync(StatusCodes.Status412PreconditionFailed, ct);
            return;
        }

        var page = await dwApiService.GetPaginatedEpisodes(req.ShowSlug, req.SeasonId, req.LastPodcastEpisodeId, req.LastShowEpisodeId, req.ShowOffset ?? 0,
            req.PodcastOffset ?? 0, req.PageNumber ?? 1, req.PageSize ?? 10, req.OrderBy ?? "CreatedAt", req.OrderDirection ?? DwSortOrderDirection.Descending,
            userInfo.Value.AccessLevel, ct);

        string? nextPageUrl = null;

        if (page.IsSuccess && !string.IsNullOrEmpty(page.Value.NextPageUrl))
        {
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host;

            var middlewareNextUrl = new Url(page.Value.NextPageUrl);
            
            var nextUrl = new Url($"{scheme}://{host}")
                .AppendPathSegment(configuration["Host:BasePath"])
                .AppendPathSegments("daily-wire", "podcasts", req.ShowSlug, "seasons", req.SeasonId, "episodes")
                .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

            var paramList = new List<string>
            {
                "lastPodcastEpisodeId",
                "lastShowEpisodeId",
                "showOffset",
                "podcastOffset",
                "pageNumber",
                "pageSize",
                "orderBy",
                "orderDirection",
            };

            foreach (var param in paramList)
            {
                if (middlewareNextUrl.QueryParams.Contains(param))
                {
                    nextUrl.SetQueryParam(param, middlewareNextUrl.QueryParams.GetAll(param));
                }
            }

            nextPageUrl = nextUrl.ToString();
        }

        var result = page.Map(MapPaginatedPage)
            .Map(p => new PodcastShowSeasonEpisodesResponse
            {
                Episodes = p.ToList(),
                NextPageUrl = nextPageUrl
            });

        await this.SendResult(result, ct);
    }

    private static IEnumerable<PodcastShowSeasonEpisodeOverview> MapPaginatedPage(DwPaginatedPage page)
    {
        foreach (var component in page.ComponentItems)
        {
            if (component is not DwShowEpisodeComponentItem showEpisode)
            {
                continue;
            }

            yield return new PodcastShowSeasonEpisodeOverview
            {
                Id = showEpisode.ShowEpisode.Id,
                Slug = showEpisode.ShowEpisode.Slug,
                Title = showEpisode.ShowEpisode.Title,
                Description = showEpisode.ShowEpisode.Description,
                Duration = showEpisode.ShowEpisode.Duration,
                MediaType = showEpisode.ShowEpisode.MediaType,
                SharingUrl = showEpisode.ShowEpisode.SharingUrl,
                ThumbnailUrl = showEpisode.ShowEpisode.Images.Thumbnail.Landscape,
                Status = showEpisode.ShowEpisode.Status,
                PublishedAt = showEpisode.ShowEpisode.PublishedAt,
                ScheduledAt = showEpisode.ShowEpisode.ScheduledAt
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