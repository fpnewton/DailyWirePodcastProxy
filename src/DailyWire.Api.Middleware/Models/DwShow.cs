using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwShow
{
    [JsonPropertyName("logoURL")]
    public string LogoUrl { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("trailerURL")]
    public string TrailerUrl { get; set; } = string.Empty;
}