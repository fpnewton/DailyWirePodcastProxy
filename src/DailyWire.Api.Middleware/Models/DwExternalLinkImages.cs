using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwExternalLinkImages
{
    [JsonPropertyName("thumbnail")]
    public DwThumbnail Thumbnail { get; set; } = null!;

    [JsonPropertyName("full")]
    public DwThumbnail Full { get; set; } = null!;

    [JsonPropertyName("credit")]
    public string Credit { get; set; } = string.Empty;
}