using MediatR;
using PodcastDatabase.Entities;

namespace PodcastProxy.Queries.GetPodcasts;

public class GetPodcastsQuery : IRequest<ICollection<Podcast>>
{
}