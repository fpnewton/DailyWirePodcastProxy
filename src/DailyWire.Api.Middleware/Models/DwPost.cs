using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwPost
{
    [JsonPropertyName("author")]
    public DwAuthor Author { get; set; } = null!;

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("images")]
    public DwImages Images { get; set; } = null!;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("publishedAt")]
    public string PublishedAt { get; set; } = string.Empty;

    [JsonPropertyName("readTime")]
    public long ReadTime { get; set; }

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("subHead")]
    public string SubHead { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("sharingURL")]
    public string SharingUrl { get; set; } = string.Empty;
}