using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Items;

public class DwExternalLinkItem : DwItem
{
    [JsonPropertyName("type")]
    public override DwItemType Type => DwItemType.ExternalLink;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("externalLink")]
    public DwExternalLink ExternalLink { get; set; } = null!;
}