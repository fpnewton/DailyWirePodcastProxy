using MediatR;
using PodcastDatabase.Entities;
using PodcastDatabase.Repositories;

namespace PodcastProxy.Queries.GetPodcasts;

public class GetPodcastsQueryHandler : IRequestHandler<GetPodcastsQuery, ICollection<Podcast>>
{
    private readonly IPodcastRepository _repository;

    public GetPodcastsQueryHandler(IPodcastRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<Podcast>> Handle(GetPodcastsQuery request, CancellationToken cancellationToken) =>
        await _repository.GetPodcasts(cancellationToken);
}