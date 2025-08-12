using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwClip
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public DwImages Images { get; set; } = null!;
}