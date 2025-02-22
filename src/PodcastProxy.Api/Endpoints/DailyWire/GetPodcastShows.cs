using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using DailyWire.Api.Models;
using DailyWire.Api.Services;
using FastEndpoints;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastShowsEndpoint(
    IConfiguration configuration,
    IDwApiService dwApiService
) : EndpointWithoutRequest<ICollection<PodcastShowOverview>>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("daily-wire/shows");
        Description(e => e.Produces<IList<PodcastShowOverview>>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var podcasts = await dwApiService.GetModularPage("listen", ct);

        var result = podcasts
            .Map(MapModularPageToPodcastOverview)
            .Map(r => r.ToList());

        await this.SendResult(result, ct);
    }

    private IEnumerable<PodcastShowOverview> MapModularPageToPodcastOverview(DwModularPageRes modularPage)
    {
        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host;

        var baseUrl = new Url($"{scheme}://{host}")
            .AppendPathSegment(configuration["Host:BasePath"])
            .AppendPathSegment("podcasts")
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        foreach (var module in modularPage.Modules)
        {
            if (module is not DwPodcastCarousel carousel)
                continue;

            foreach (var podcast in carousel.Podcasts)
            {
                yield return new PodcastShowOverview
                {
                    Id = podcast.Id,
                    Slug = podcast.Slug,
                    Name = podcast.Name,
                    Description = podcast.Description,
                    Feed = baseUrl.Clone().AppendPathSegments(podcast.Id, "feed").ToUri()
                };
            }
        }
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PodcastShowOverview
{
    public string Id { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Uri Feed { get; set; } = default!;
}