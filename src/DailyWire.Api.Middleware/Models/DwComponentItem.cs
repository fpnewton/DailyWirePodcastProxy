using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Converters;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models;

[JsonConverter(typeof(DwComponentItemConverter))] 
public abstract class DwComponentItem
{
    [JsonPropertyName("type")]
    public abstract DwComponentItemType Type { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nextPageUrl")]
    public string? NextPageUrl { get; set; }
}
