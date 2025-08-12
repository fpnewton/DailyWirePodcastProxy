using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwShowPageShow
{
    [JsonPropertyName("author")]
    public DwAuthor Author { get; set; } = null!;

    [JsonPropertyName("backgroundImage")]
    public string BackgroundImage { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("fansCount")]
    public string FansCount { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("images")]
    public DwImages Images { get; set; } = null!;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("latestEpisode")]
    public DwEpisode LatestEpisode { get; set; } = null!;

    [JsonPropertyName("logoImage")]
    public string LogoImage { get; set; } = string.Empty;

    [JsonPropertyName("matureRating")]
    public string MatureRating { get; set; } = string.Empty;

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public string Rating { get; set; } = string.Empty;

    [JsonPropertyName("seasons")]
    public List<DwEntity> Seasons { get; set; } = [];

    [JsonPropertyName("sharingURL")]
    public string SharingUrl { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("trailerURL")]
    public string TrailerUrl { get; set; } = string.Empty;
}