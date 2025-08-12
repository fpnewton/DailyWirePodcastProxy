using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwAuthor
{
    [JsonPropertyName("authorIsHost")]
    public bool AuthorIsHost { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("headshot")]
    public string Headshot { get; set; } = string.Empty;
}