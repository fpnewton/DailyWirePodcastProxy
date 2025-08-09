namespace DailyWire.Api.Models;

[Obsolete]
public class DwShowCarousel : IDwModule
{
    public string Typename => "ShowCarousel";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Orientation { get; set; }
    public IList<DwShow> Shows { get; set; } = new List<DwShow>();
}