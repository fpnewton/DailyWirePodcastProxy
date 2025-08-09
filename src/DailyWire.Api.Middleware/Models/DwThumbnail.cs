using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwThumbnail
{
    [JsonPropertyName("land")]
    public string Landscape { get; set; } = string.Empty;

    [JsonPropertyName("port")]
    public string Portrait { get; set; } = string.Empty;

    [JsonPropertyName("square")]
    public string Square { get; set; } = string.Empty;
}