using Newtonsoft.Json;

namespace DailyWire.Api.Models;

public class DwGetPodcastEpisodeRes
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;

    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Audio { get; set; }
    public DwSeasonDetails? Season { get; set; }
    public double? ListenTime { get; set; }
    public IList<string> AllowedContinents = new List<string>();
    public string? Thumbnail { get; set; }
    public double? Duration { get; set; }
    public double? Rating { get; set; }
    public string? Status { get; set; }
    public string? AudioState { get; set; }
    public DwGetPodcastRes? Podcast { get; set; }
    public DateTimeOffset? PublishDate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? ScheduleAt { get; set; }
}