using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using DailyWire.Api.Models;
using DailyWire.Api.Services;
using FastEndpoints;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;
using QRCoder;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastsEndpoint(
    IConfiguration configuration,
    IDwApiService dwApiService
) : EndpointWithoutRequest<ICollection<PodcastOverview>>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("daily-wire/podcasts");
        Description(e => e.Produces<IList<PodcastOverview>>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var podcasts = await dwApiService.GetModularPage("listen", ct);

        var result = podcasts
            .Map(MapModularPageToPodcastOverview)
            .Map(r => r.ToList());

        await this.SendResult(result, ct);
    }

    private IEnumerable<PodcastOverview> MapModularPageToPodcastOverview(DwModularPageRes modularPage)
    {
        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host;

        var baseUrl = new Url($"{scheme}://{host}")
            .AppendPathSegment(HttpContext.Request.PathBase)
            .AppendPathSegment(HttpContext.Request.Path)
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        foreach (var module in modularPage.Modules)
        {
            if (module is not DwPodcastCarousel carousel)
                continue;

            foreach (var podcast in carousel.Podcasts)
            {
                yield return new PodcastOverview
                {
                    Id = podcast.Id,
                    Slug = podcast.Slug,
                    Name = podcast.Name,
                    Description = podcast.Description,
                    Feed = baseUrl.AppendPathSegments(podcast.Id, "feed").ToUri()
                };
            }
        }
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PodcastOverview
{
    public string Id { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Uri Feed { get; set; } = default!;
}