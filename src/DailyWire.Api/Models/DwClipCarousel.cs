namespace DailyWire.Api.Models;

[Obsolete]
public class DwClipCarousel : IDwModule
{
    public string Typename => "ClipCarousel";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Layout { get; set; }
    public IList<DwClip> Clips { get; set; } = new List<DwClip>();
}