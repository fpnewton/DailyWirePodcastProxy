using Ardalis.Result;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Api.Extensions;
using PodcastProxy.Application.Commands.Podcasts;
using PodcastProxy.Application.Queries.Podcasts;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class CheckPodcastForNewEpisodesRequest
{
    public required string PodcastId { get; set; }
}

public class CheckPodcastForNewEpisodesEndpoint : Endpoint<CheckPodcastForNewEpisodesRequest>
{
    public override void Configure()
    {
        Get("podcasts/{PodcastId}/new-episodes");
        Description(d => d.Produces(StatusCodes.Status200OK));
    }

    public override async Task HandleAsync(CheckPodcastForNewEpisodesRequest req, CancellationToken ct)
    {
        var podcast = await new GetPodcastByIdQuery { PodcastId = req.PodcastId }.ExecuteAsync(ct);

        if (!podcast.IsSuccess)
        {
            await this.SendResult(podcast.Map(), ct);
            return;
        }

        await new CheckPodcastForNewEpisodesCommand { PodcastSlug = podcast.Value.Slug }.ExecuteAsync(ct);

        await Send.OkAsync(ct);
    }
}