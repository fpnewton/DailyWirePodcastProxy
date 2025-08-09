namespace DailyWire.Api.Models;

[Obsolete]
public class DwShow : IDwItem
{
    public string Typename => "Show";
    
    public string Id { get; set; } = null!;
    
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; set; }
    public string? Image { get; set; }
    public string? BelongsTo { get; set; }
    public string? PortraitImage { get; set; }
    public string? BackgroundImage { get; set; }
    public string? LogoImage { get; set; }
}