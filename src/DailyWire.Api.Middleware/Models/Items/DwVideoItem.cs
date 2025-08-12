using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Items;

public class DwVideoItem : DwItem
{
    [JsonPropertyName("type")]
    public override DwItemType Type => DwItemType.Video;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("video")]
    public DwVideo Video { get; set; } = null!;
}