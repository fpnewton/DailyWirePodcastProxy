using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Items;

public class DwClipItem : DwItem
{
    [JsonPropertyName("type")]
    public override DwItemType Type => DwItemType.Clip;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("clip")]
    public DwClip Clip { get; set; } = null!;
}