namespace DailyWire.Api.Models;

[Obsolete]
public class DwVideoCarousel : IDwModule
{
    public string Typename => "VideoCarousel";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Layout { get; set; }
    public IList<DwVideo> Videos { get; set; } = new List<DwVideo>();
}