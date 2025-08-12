using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwEpisodeShow
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public DwAuthor Author { get; set; } = null!;

    [JsonPropertyName("logoURL")]
    public string LogoUrl { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("matureRating")]
    public string MatureRating { get; set; } = string.Empty;

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public string Rating { get; set; } = string.Empty;
}