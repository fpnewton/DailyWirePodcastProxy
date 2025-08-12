using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwHost
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("author")]
    public DwAuthor Author { get; set; } = null!;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}