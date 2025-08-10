using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class EpisodeBySeasonIdSpec : Specification<Episode>
{
    public EpisodeBySeasonIdSpec(string seasonId)
    {
        Query
            .Where(episode => string.Equals(episode.SeasonId, seasonId))
            .AsNoTracking();
    }
}