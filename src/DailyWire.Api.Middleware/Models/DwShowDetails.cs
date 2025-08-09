using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwShowDetails
{
    [JsonPropertyName("backgroundImage")]
    public string BackgroundImage { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("host")]
    public DwEntity Host { get; set; } = null!;

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("images")]
    public DwImages Images { get; set; } = null!;

    [JsonPropertyName("logoImage")]
    public string LogoImage { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}