namespace DailyWire.Api.Models;

public class DwPodcastCarousel : IDwModule
{
    public string Typename => "PodcastCarousel";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public IList<DwPodcast> Podcasts { get; set; } = new List<DwPodcast>();
}