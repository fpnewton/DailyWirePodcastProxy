namespace PodcastDatabase.Entities;

public class Podcast
{
    public string Id { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public Uri? CoverImage { get; set; }
    public Uri? BackgroundImage { get; set; }
    public Uri? LogoImage { get; set; }
    public string? BelongsTo { get; set; }

    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}