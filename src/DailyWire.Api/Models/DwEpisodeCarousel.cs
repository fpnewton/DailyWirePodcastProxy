namespace DailyWire.Api.Models;

public class DwEpisodeCarousel : IDwModule
{
    public string Typename => "EpisodeCarousel";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;

    public string? Title { get; set; }
    public int? Order { get; set; }
    public IList<DwEpisode> Episodes { get; set; } = new List<DwEpisode>();
}