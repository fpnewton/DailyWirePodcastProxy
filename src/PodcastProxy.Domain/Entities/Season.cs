using PodcastProxy.Domain.Interfaces;

namespace PodcastProxy.Domain.Entities;

public class Season : IEntity
{
    public string SeasonId { get; set; } = null!;
    public string PodcastId { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }

    public Podcast Podcast { get; set; } = null!;
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    private sealed class SeasonIdPodcastIdEqualityComparer : IEqualityComparer<Season>
    {
        public bool Equals(Season? x, Season? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            
            return x.SeasonId == y.SeasonId && x.PodcastId == y.PodcastId;
        }

        public int GetHashCode(Season obj)
        {
            return HashCode.Combine(obj.SeasonId, obj.PodcastId);
        }
    }

    public static IEqualityComparer<Season> DefaultComparer { get; } = new SeasonIdPodcastIdEqualityComparer();
}