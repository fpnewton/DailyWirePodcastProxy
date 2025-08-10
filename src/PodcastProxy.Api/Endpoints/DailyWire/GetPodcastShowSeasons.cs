using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using FastEndpoints;
using PodcastProxy.Api.Extensions;
using PodcastProxy.Application.Queries.Shows;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastShowSeasonsRequest
{
    public string PodcastSlug { get; set; } = string.Empty;
}

public class GetPodcastShowSeasonsEndpoint : Endpoint<GetPodcastShowSeasonsRequest>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("daily-wire/podcasts/{PodcastSlug}/seasons");
    }

    public override async Task HandleAsync(GetPodcastShowSeasonsRequest req, CancellationToken ct)
    {
        var seasons = await new GetShowSeasonsQuery { ShowSlug = req.PodcastSlug }.ExecuteAsync(ct);

        var result = seasons
            .Map(MapSeasonDetailsToPodcastSeasonOverview)
            .Map(r => r.ToList());

        await this.SendResult(result, ct);
    }

    private static IEnumerable<PodcastShowSeasonOverview> MapSeasonDetailsToPodcastSeasonOverview(IList<DwEntity> seasons) => seasons
        .Select(season => new PodcastShowSeasonOverview
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