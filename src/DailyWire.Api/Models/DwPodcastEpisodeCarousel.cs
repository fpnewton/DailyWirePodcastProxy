namespace DailyWire.Api.Models;

public class DwPodcastEpisodeCarousel : IDwModule
{
    public string Typename => "PodcastEpisodeCarousel";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public IList<DwPodcastEpisode> PodcastEpisodes { get; set; } = new List<DwPodcastEpisode>();
}