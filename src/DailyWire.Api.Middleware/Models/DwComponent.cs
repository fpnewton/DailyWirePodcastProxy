using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Converters;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models;

[JsonConverter(typeof(DwComponentConverter))] 
public abstract class DwComponent
{
    [JsonPropertyName("renderType")]
    public abstract DwComponentRenderType RenderType { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nextPageUrl")]
    public string? NextPageUrl { get; set; }
}