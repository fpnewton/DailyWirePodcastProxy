using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwPage
{
    [JsonPropertyName("components")]
    public List<DwComponent> Components { get; set; } = [];

    [JsonPropertyName("contentFilters")]
    public List<DwContentFilter> ContentFilters { get; set; } = [];

    [JsonPropertyName("pageName")]
    public required string PageName { get; set; }
}