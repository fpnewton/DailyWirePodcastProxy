namespace DailyWire.Api.Models;

[Obsolete]
public class DwPodcast
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? BelongsTo { get; set; }
    public string? LogoImage { get; set; }
    public string? CoverImage { get; set; }
    public string? BackgroundImage { get; set; }
    public DwUser? Author { get; set; }
}