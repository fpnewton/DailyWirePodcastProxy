namespace DailyWire.Api.Models;

[Obsolete]
public class DwEpisode
{
    public string Id { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Image { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DwShow? Show { get; set; }
}