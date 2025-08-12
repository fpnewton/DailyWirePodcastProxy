using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using DailyWire.Api.Middleware.Models.Items;
using FastEndpoints;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;
using PodcastProxy.Application.Queries.Shows;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastShowsEndpoint(IConfiguration configuration) : EndpointWithoutRequest<ICollection<PodcastShowOverview>>
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
        var shows = await new GetShowsQuery().ExecuteAsync(ct);

        var result = shows
            .Map(MapShowItemsToPodcastOverviews)
            .Map(o => o.ToList());

        await this.SendResult(result, ct);
    }

    private IEnumerable<PodcastShowOverview> MapShowItemsToPodcastOverviews(IList<DwShowItem> shows)
    {
        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host;

        var baseUrl = new Url($"{scheme}://{host}")
            .AppendPathSegment(configuration["Host:BasePath"])
            .AppendPathSegment("podcasts")
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        foreach (var show in shows)
        {
            yield return new PodcastShowOverview
            {
                Id = show.Show.Id,
                Slug = show.Show.Slug,
                Name = show.Show.Title,
                Description = show.Show.Description,
                Feed = baseUrl.Clone().AppendPathSegments(show.Show.Id, "feed").ToUri()
            };
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
    public Uri Feed { get; set; } = null!;
}