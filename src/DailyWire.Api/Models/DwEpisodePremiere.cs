namespace DailyWire.Api.Models;

[Obsolete]
public class DwEpisodePremiere : IDwModule
{
    public string Typename => "EpisodePremiere";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;

    public string? PremiereImage { get; set; }
    public DwEpisode? Episode { get; set; }
}