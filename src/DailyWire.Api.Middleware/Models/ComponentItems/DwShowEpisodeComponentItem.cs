using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.ComponentItems;

public class DwShowEpisodeComponentItem : DwComponentItem
{
    public override DwComponentItemType Type => DwComponentItemType.ShowEpisode;

    [JsonPropertyName("badgeText")]
    public string? BadgeText { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("showEpisode")]
    public DwShowEpisode ShowEpisode { get; set; } = null!;
}