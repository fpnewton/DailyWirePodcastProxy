using MediatR;
using PodcastProxy.Application.Commands.FetchLatestEpisodes;
using PodcastProxy.Application.Queries;

namespace PodcastProxy.Application.Commands.CheckPodcastNewEpisodes;

public class CheckPodcastNewEpisodesCommandHandler(IMediator mediator) : IRequestHandler<CheckPodcastNewEpisodesCommand>
{
    public async Task Handle(CheckPodcastNewEpisodesCommand request, CancellationToken cancellationToken)
    {
        var query = new GetLatestSeasonForPodcastQuery { PodcastId = request.PodcastId };
        var season = await mediator.Send(query, cancellationToken);

        if (season.IsSuccess)
        {
            var command = new FetchLatestEpisodesCommand { SeasonId = season.Value.SeasonId, First = 3 };

            await mediator.Send(command, cancellationToken);
        }
    }
}