using MediatR;
using PodcastDatabase.Entities;
using PodcastDatabase.Repositories;

namespace PodcastProxy.Queries.GetLatestSeasonForPodcast;

public class GetLatestSeasonForPodcastQueryHandler : IRequestHandler<GetLatestSeasonForPodcastQuery, Season?>
{
    private readonly ISeasonRepository _repository;

    public GetLatestSeasonForPodcastQueryHandler(ISeasonRepository repository)
    {
        _repository = repository;
    }

    public async Task<Season?> Handle(GetLatestSeasonForPodcastQuery request, CancellationToken cancellationToken) =>
        await _repository.GetLatestSeasonForPodcast(request.PodcastId, cancellationToken);
}