namespace DailyWire.Api.Models;

[Obsolete]
public class DwVideo
{
    public string Id { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Image { get; set; }
    public string? LogoImage { get; set; }
}