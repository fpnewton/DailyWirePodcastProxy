using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class PodcastSpec : Specification<Podcast>
{
    public PodcastSpec()
    {
        Query
            .AsNoTracking()
            .OrderBy(podcast => podcast.Name);
    }
}