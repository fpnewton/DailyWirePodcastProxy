using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwHeader
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }
}