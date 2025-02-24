using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Application.Commands.CheckAllNewEpisodes;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class CheckPodcastsForNewEpisodes(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("podcasts/new-episodes");
        Description(d => d.Produces(StatusCodes.Status200OK));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new CheckAllNewEpisodesCommand();
        
        await mediator.Send(command, ct);

        await SendOkAsync(ct);
    }
}