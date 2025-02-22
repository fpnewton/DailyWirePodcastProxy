using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class EpisodeByIdSpec : Specification<Episode>
{
    public EpisodeByIdSpec(string episodeId)
    {
        Query.Where(episode => string.Equals(episode.EpisodeId, episodeId));
    }
}