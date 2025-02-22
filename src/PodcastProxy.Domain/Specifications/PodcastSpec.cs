using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class PodcastSpec : Specification<Podcast>
{
    public PodcastSpec()
    {
        Query.OrderBy(podcast => podcast.Name);
    }
}