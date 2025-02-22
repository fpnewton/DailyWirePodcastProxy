using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class PodcastBySlugSpec : Specification<Podcast>
{
    public PodcastBySlugSpec(string slug)
    {
        Query.Where(podcast => string.Equals(podcast.Slug, slug));
    }
}

public static class PodcastBySlugSpecExtensions
{
    public static PodcastBySlugSpec WithEpisodes(this PodcastBySlugSpec spec)
    {
        spec.Query
            .Include(e => e.Seasons.OrderByDescending(s => s.Slug))
            .ThenInclude(e => e.Episodes.OrderByDescending(s => s.Slug));

        return spec;
    }
}