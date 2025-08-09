using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.Components;
using DailyWire.Api.Middleware.Models.Items;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastShowsEndpoint(
    IConfiguration configuration,
    IDailyWireMiddlewareApi dwApiService
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
        var userInfo = await dwApiService.GetUserInfo(ct);
        
        if (!userInfo.IsSuccess)
        {
            AddError("Failed to get user info.");

            await SendErrorsAsync(StatusCodes.Status412PreconditionFailed, ct);
            return;
        }

        var page = await dwApiService.GetPage("watch-page", userInfo.Value.AccessLevel, ct);

        var result = page
            .Map(MapPageToPodcastOverview)
            .Map(r => r.ToList());

        await this.SendResult(result, ct);
    }

    private IEnumerable<PodcastShowOverview> MapPageToPodcastOverview(DwPage page)
    {
        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host;

        var baseUrl = new Url($"{scheme}://{host}")
            .AppendPathSegment(configuration["Host:BasePath"])
            .AppendPathSegment("podcasts")
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        foreach (var component in page.Components)
        {
            if (component is not DwSquareShowCarouselComponent showCarousel)
            {
                continue;
            }
            
            foreach (var item in showCarousel.Items)
            {
                if (item is not DwShowItem show)
                {
                    continue;
                }
            
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