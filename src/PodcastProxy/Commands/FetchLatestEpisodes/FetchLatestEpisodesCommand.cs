using AutoMapper;
using DailyWireApi.Queries.ListPodcastEpisode;
using MediatR;
using PodcastDatabase.Entities;
using PodcastDatabase.Repositories;

namespace PodcastProxy.Commands.FetchLatestEpisodes;

public class FetchLatestEpisodesCommand : IRequest<ICollection<Episode>>
{
    public string SeasonId { get; set; } = null!;
    public int First { get; set; } = 10;
}

public class FetchLatestEpisodesCommandHandler : IRequestHandler<FetchLatestEpisodesCommand, ICollection<Episode>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IEpisodeRepository _repository;

    public FetchLatestEpisodesCommandHandler(IMapper mapper, IMediator mediator, IEpisodeRepository repository)
    {
        _mapper = mapper;
        _mediator = mediator;
        _repository = repository;
    }

    public async Task<ICollection<Episode>> Handle(FetchLatestEpisodesCommand request, CancellationToken cancellationToken)
    {
        var query = new ListPodcastEpisodeQuery { SeasonId = request.SeasonId, First = request.First, Skip = 0 };
        var models = await _mediator.Send(query, cancellationToken);
        var episodes = new List<Episode>(models.Count);

        foreach (var model in models)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                continue;
            }

            if (!string.Equals(model.Status, "PUBLISHED", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var exists = await _repository.EpisodeExists(model.Id, cancellationToken);
            var episode = _mapper.Map<Episode>(model);

            episode.SeasonId = request.SeasonId;

            if (!exists)
            {
                episode = await _repository.InsertEpisode(episode, cancellationToken);
            }
            else
            {
                episode = await _repository.UpdateEpisode(episode, cancellationToken);
            }

            episodes.Add(episode);
        }

        return episodes;
    }
}