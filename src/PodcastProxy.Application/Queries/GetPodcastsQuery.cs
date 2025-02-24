using MediatR;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries;

public class GetPodcastsQuery : IRequest<ICollection<Podcast>>;

public class GetPodcastsQueryHandler(IRepository<Podcast> repository) : IRequestHandler<GetPodcastsQuery, ICollection<Podcast>>
{
    public async Task<ICollection<Podcast>> Handle(GetPodcastsQuery request, CancellationToken cancellationToken)
    {
        var spec = new PodcastSpec();

        return await repository.ListAsync(spec, cancellationToken);
    }
}