namespace DailyWire.Api.Models;

[Obsolete]
public class DwPodcastEpisodePremiere : IDwModule
{
    public string Typename => "PodcastEpisodePremiere";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? PremiereImage { get; set; }
    public DwPodcastEpisode? PodcastEpisode { get; set; }
}