namespace DailyWire.Api.Models;

public class DwPodcastPremiere : IDwModule
{
    public string Typename => "PodcastPremiere";

    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;

    public string? PremiereImage { get; set; }
    public DwPodcast? Podcast { get; set; }
}