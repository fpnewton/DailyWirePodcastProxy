using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwImages
{
    [JsonPropertyName("thumbnail")]
    public DwThumbnail Thumbnail { get; set; } = null!;

    [JsonPropertyName("credit")]
    public string Credit { get; set; } = string.Empty;
    
    [JsonPropertyName("publishedAt")]
    public DateTime? PublishedAt { get; set; }
}