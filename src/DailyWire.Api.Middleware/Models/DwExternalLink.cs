using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwExternalLink
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public DwExternalLinkImages Images { get; set; } = null!;

    [JsonPropertyName("url")]
    public string Url { get; set; } =  string.Empty;
}