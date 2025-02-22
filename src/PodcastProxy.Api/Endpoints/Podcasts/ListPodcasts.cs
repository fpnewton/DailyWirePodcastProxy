using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Application.Queries;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class ListPodcastsEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("/podcasts");
        Description(d => d.Produces<ICollection<Podcast>>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetPodcastsQuery();
        var result = await mediator.Send(query, ct);

        await SendOkAsync(result, ct);
    }
}