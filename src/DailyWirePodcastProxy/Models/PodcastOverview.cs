namespace DailyWirePodcastProxy.Models;

public class PodcastOverview
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Uri Feed { get; set; } = null!;
}