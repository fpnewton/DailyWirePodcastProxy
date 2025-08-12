using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwEpisode
{
    [JsonPropertyName("availableFor")]
    public List<string> AvailableFor { get; set; } = [];

    [JsonPropertyName("backgroundImage")]
    public string BackgroundImage { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("episodeNumber")]
    public string EpisodeNumber { get; set; } = string.Empty;

    [JsonPropertyName("host")]
    public DwEntity Host { get; set; } = null!;

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("images")]
    public DwImages Images { get; set; } = null!;

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("parentTitle")]
    public string ParentTitle { get; set; } = string.Empty;

    [JsonPropertyName("publishedAt")]
    public DateTimeOffset PublishedAt { get; set; }

    [JsonPropertyName("scheduledAt")]
    public DateTimeOffset ScheduledAt { get; set; }

    [JsonPropertyName("sharingURL")]
    public string SharingUrl { get; set; } = string.Empty;

    [JsonPropertyName("show")]
    public DwLatestEpisodeShow Show { get; set; } = null!;

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}