using Ardalis.Specification;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Domain.Specifications;

public sealed class SeasonByIdSpec : Specification<Season>
{
    public SeasonByIdSpec(string seasonId)
    {
        Query
            .Where(season => string.Equals(season.SeasonId, seasonId))
            .AsNoTracking();
    }
}