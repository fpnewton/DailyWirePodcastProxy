using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwTab
{
    [JsonPropertyName("components")]
    public List<DwComponent> Components { get; set; } = [];

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}