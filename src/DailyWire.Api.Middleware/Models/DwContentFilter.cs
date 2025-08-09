using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwContentFilter
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }
}