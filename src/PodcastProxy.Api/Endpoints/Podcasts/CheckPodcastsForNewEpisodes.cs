using FastEndpoints;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Application.Commands.Podcasts;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class CheckPodcastsForNewEpisodes : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("podcasts/new-episodes");
        Description(d => d.Produces(StatusCodes.Status200OK));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await new CheckAllPodcastsForNewEpisodesCommand().ExecuteAsync(ct);

        await Send.OkAsync(ct);
    }
}