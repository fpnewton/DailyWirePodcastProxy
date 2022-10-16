namespace PodcastDatabase.Entities;

public class Season
{
    public string SeasonId { get; set; } = null!;
    public string PodcastId { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }

    public Podcast Podcast { get; set; } = null!;
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}