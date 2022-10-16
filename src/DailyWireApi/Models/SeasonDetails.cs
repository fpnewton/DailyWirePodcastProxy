using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class SeasonDetails
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;

    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}