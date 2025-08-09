using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Services;
using DailyWire.Api.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastShowSeasonsRequest
{
    public string PodcastSlug { get; set; } = string.Empty;
}

public class GetPodcastShowSeasonsEndpoint(IDailyWireMiddlewareApi dwApiService) : Endpoint<GetPodcastShowSeasonsRequest>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("daily-wire/podcasts/{PodcastSlug}/seasons");
    }

    [ProducesResponseType(typeof(IList<DwSeasonDetails>), StatusCodes.Status200OK)]
    public override async Task HandleAsync(GetPodcastShowSeasonsRequest req, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
        {
            AddError("Failed to get user info.");

            await SendErrorsAsync(StatusCodes.Status412PreconditionFailed, ct);
            return;
        }

        var page = await dwApiService.GetShowPage(req.PodcastSlug, userInfo.Value.AccessLevel, ct);

        var result = page
            .Map(MapSeasonDetailsToPodcastSeasonOverview)
            .Map(r => r.ToList());

        await this.SendResult(result, ct);
    }

    private static IEnumerable<PodcastShowSeasonOverview> MapSeasonDetailsToPodcastSeasonOverview(DwShowPage details) =>
        details.Show.Seasons.Select(season => new PodcastShowSeasonOverview
        {
            Id = season.Id,
            Slug = season.Slug,
            Name = season.Name,
            Description = season.Name
        });
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PodcastShowSeasonOverview
{
    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
}