using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class SeasonByPodcastIdSpec : Specification<Season>
{
    public SeasonByPodcastIdSpec(string podcastId)
    {
        Query
            .Where(season => string.Equals(season.PodcastId, podcastId))
            .AsNoTracking()
            .OrderByDescending(e => e.Name);
    }
}