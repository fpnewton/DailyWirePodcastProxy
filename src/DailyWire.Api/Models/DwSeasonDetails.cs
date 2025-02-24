using Newtonsoft.Json;

namespace DailyWire.Api.Models;

public class DwSeasonDetails
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;

    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
}