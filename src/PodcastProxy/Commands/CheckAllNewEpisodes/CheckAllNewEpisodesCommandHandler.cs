using MediatR;
using PodcastProxy.Commands.CheckPodcastNewEpisodes;
using PodcastProxy.Queries.GetPodcasts;

namespace PodcastProxy.Commands.CheckAllNewEpisodes;

public class CheckAllNewEpisodesCommandHandler : IRequestHandler<CheckAllNewEpisodesCommand>
{
    private readonly IMediator _mediator;

    public CheckAllNewEpisodesCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(CheckAllNewEpisodesCommand request, CancellationToken cancellationToken)
    {
        var query = new GetPodcastsQuery();
        var podcasts = await _mediator.Send(query, cancellationToken);

        foreach (var podcast in podcasts)
        {
            var command = new CheckPodcastNewEpisodesCommand { PodcastId = podcast.Id };

            await _mediator.Send(command, cancellationToken);
        }
        
        return Unit.Value;
    }
}