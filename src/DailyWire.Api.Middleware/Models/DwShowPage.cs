using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwShowPage
{
    [JsonPropertyName("selectedSeason")]
    public DwEntity SelectedSeason { get; set; } = null!;

    [JsonPropertyName("show")]
    public DwShowPageShow Show { get; set; } = null!;

    [JsonPropertyName("tabs")]
    public List<DwTab> Tabs { get; set; } = [];
}