using MediatR;
using PodcastProxy.Application.Commands.CheckPodcastNewEpisodes;
using PodcastProxy.Application.Queries;

namespace PodcastProxy.Application.Commands.CheckAllNewEpisodes;

public class CheckAllNewEpisodesCommandHandler(IMediator mediator) : IRequestHandler<CheckAllNewEpisodesCommand>
{
    public async Task Handle(CheckAllNewEpisodesCommand request, CancellationToken cancellationToken)
    {
        var query = new GetPodcastsQuery();
        var podcasts = await mediator.Send(query, cancellationToken);

        foreach (var podcast in podcasts)
        {
            var command = new CheckPodcastNewEpisodesCommand { PodcastId = podcast.Id };

            await mediator.Send(command, cancellationToken);
        }
    }
}