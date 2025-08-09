using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Items;

public class DwHostItem : DwItem
{
    [JsonPropertyName("type")]
    public override DwItemType Type => DwItemType.Host;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("host")]
    public DwHost Host { get; set; } = null!;
}