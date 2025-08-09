using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Converters;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models;

[JsonConverter(typeof(DwItemConverter))]
public abstract class DwItem
{
    [JsonPropertyName("type")]
    public abstract DwItemType Type { get; }

    [JsonPropertyName("badgeText")]
    public string? BadgeText { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("ctaText")]
    public string? CallToActionText { get; set; }
}
