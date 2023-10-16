using MediatR;
using PodcastProxy.Commands.FetchLatestEpisodes;
using PodcastProxy.Queries.GetLatestSeasonForPodcast;

namespace PodcastProxy.Commands.CheckPodcastNewEpisodes;

public class CheckPodcastNewEpisodesCommandHandler : IRequestHandler<CheckPodcastNewEpisodesCommand>
{
    private readonly IMediator _mediator;

    public CheckPodcastNewEpisodesCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CheckPodcastNewEpisodesCommand request, CancellationToken cancellationToken)
    {
        var query = new GetLatestSeasonForPodcastQuery { PodcastId = request.PodcastId };
        var season = await _mediator.Send(query, cancellationToken);

        if (season is not null)
        {
            var command = new FetchLatestEpisodesCommand { SeasonId = season.SeasonId, First = 3 };

            await _mediator.Send(command, cancellationToken);
        }
    }
}