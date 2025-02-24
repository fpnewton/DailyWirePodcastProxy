namespace DailyWire.Api.Models;

public class DwPodcastEpisode
{
    public string Id { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; set; }
    public string? Status { get; set; }
    public double? Duration { get; set; }
    public string? Audio { get; set; }
    public DateTimeOffset? PublishDate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DwPodcast? Podcast { get; set; }
}