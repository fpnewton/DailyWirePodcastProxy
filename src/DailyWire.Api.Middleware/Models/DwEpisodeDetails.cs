using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models;

public class DwEpisodeDetails
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("slug")]
    public required string Slug { get; set; }
    
    [JsonPropertyName("episodeNumber")]
    public string EpisodeNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public DwImages Images { get; set; } = null!;

    [JsonPropertyName("publishedAt")]
    public DateTimeOffset PublishedAt { get; set; }

    [JsonPropertyName("show")]
    public DwEpisodeShow Show { get; set; } = null!;

    [JsonPropertyName("segmentAudioURL")]
    public string SegmentAudioUrl { get; set; } = string.Empty;

    [JsonPropertyName("audioURL")]
    public string AudioUrl { get; set; } = string.Empty;

    [JsonPropertyName("videoURL")]
    public string VideoUrl { get; set; } = string.Empty;

    [JsonPropertyName("secureVideoURL")]
    public string SecureVideoUrl { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("progress")]
    public long Progress { get; set; }

    [JsonPropertyName("status")]
    public DwStatus Status { get; set; }

    [JsonPropertyName("deliveryMode")]
    public string DeliveryMode { get; set; } = string.Empty;

    [JsonPropertyName("scheduledAt")]
    public DateTimeOffset ScheduledAt { get; set; }

    [JsonPropertyName("availableFor")]
    public List<string> AvailableFor { get; set; } = [];

    [JsonPropertyName("chatAvailableFor")]
    public List<string> ChatAvailableFor { get; set; } = [];

    [JsonPropertyName("sharingURL")]
    public string SharingUrl { get; set; } = string.Empty;

    [JsonPropertyName("continueWatchingEntityType")]
    public string ContinueWatchingEntityType { get; set; } = string.Empty;

    [JsonPropertyName("continueWatchingEntityId")]
    public string ContinueWatchingEntityId { get; set; } = string.Empty;

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("playbackPolicy")]
    public string PlaybackPolicy { get; set; } = string.Empty;

    [JsonPropertyName("nextEpisodeURL")]
    public string NextEpisodeUrl { get; set; } = string.Empty;
}