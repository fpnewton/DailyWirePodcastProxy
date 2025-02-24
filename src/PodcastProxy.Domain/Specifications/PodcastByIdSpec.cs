using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class PodcastByIdSpec : SingleResultSpecification<Podcast>
{
    public PodcastByIdSpec(string podcastId)
    {
        Query
            .Where(podcast => string.Equals(podcast.Id, podcastId))
            .AsNoTracking()
            .Include(podcast => podcast.Seasons)
            .ThenInclude(season => season.Episodes)
            .AsNoTracking();
    }

    public PodcastByIdSpec(Podcast podcast) : this(podcast.Id)
    {
    }
}