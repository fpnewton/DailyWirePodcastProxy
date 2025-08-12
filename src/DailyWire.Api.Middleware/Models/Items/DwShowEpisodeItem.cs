using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Items;

public class DwShowEpisodeItem : DwItem
{
    [JsonPropertyName("type")]
    public override DwItemType Type => DwItemType.ShowEpisode;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("showEpisode")]
    public DwShowEpisode ShowEpisode { get; set; } = null!;
}