using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using DailyWire.Api.Models;
using DailyWire.Api.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastSeasons
{
    public string PodcastSlug { get; set; } = string.Empty;
}

public class GetPodcastSeasonsEndpoint(IDwApiService dwApiService) : Endpoint<GetPodcastSeasons>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("daily-wire/podcasts/{PodcastSlug}/seasons");
    }

    [ProducesResponseType(typeof(IList<DwSeasonDetails>), StatusCodes.Status200OK)]
    public override async Task HandleAsync(GetPodcastSeasons req, CancellationToken ct)
    {
        var seasons = await dwApiService.GetPodcastSeasonsBySlug(req.PodcastSlug, ct);

        var result = seasons
            .Map(MapSeasonDetailsToPodcastSeasonOverview)
            .Map(r => r.ToList());

        await this.SendResult(result, ct);
    }

    private static IEnumerable<PodcastSeasonOverview> MapSeasonDetailsToPodcastSeasonOverview(IEnumerable<DwSeasonDetails> details) =>
        details.Select(detail => new PodcastSeasonOverview
        {
            Id = detail.Id,
            Slug = detail.Slug,
            Name = detail.Name,
            Description = detail.Description
        });
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PodcastSeasonOverview
{
    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
}