using FastEndpoints;
using Flurl;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Application.Queries;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class ListPodcastsEndpoint(
    IConfiguration configuration,
    IMediator mediator
) : EndpointWithoutRequest
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("/podcasts");
        Description(d => d.Produces<IList<PodcastResponse>>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetPodcastsQuery();
        var result = await mediator.Send(query, ct);
        var response = MapPodcastsResponse(result).ToList();

        await SendOkAsync(response, ct);
    }

    private IEnumerable<PodcastResponse> MapPodcastsResponse(ICollection<Podcast> podcasts)
    {
        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host;

        var baseUrl = new Url($"{scheme}://{host}")
            .AppendPathSegment(configuration["Host:BasePath"])
            .AppendPathSegment("podcasts")
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        foreach (var podcast in podcasts)
        {
            yield return new PodcastResponse
            {
                Id = podcast.Id,
                Slug = podcast.Slug,
                Name = podcast.Name,
                Description = podcast.Description,
                Status = podcast.Status,
                CoverImage = podcast.CoverImage,
                BackgroundImage = podcast.BackgroundImage,
                LogoImage = podcast.LogoImage,
                BelongsTo = podcast.BelongsTo,
                Seasons = podcast.Seasons,
                Feed = baseUrl.Clone().AppendPathSegments(podcast.Id, "feed").ToUri()
            };
        }
    }
}

public class PodcastResponse : Podcast
{
    public Uri Feed { get; set; } = null!;
}