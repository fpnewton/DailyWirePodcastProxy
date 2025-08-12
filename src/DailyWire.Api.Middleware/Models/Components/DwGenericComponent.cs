using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Components;

public class DwGenericComponent : DwComponent
{
    public override DwComponentRenderType RenderType => DwComponentRenderType.Generic;

    [JsonPropertyName("items")]
    public List<DwItem> Items { get; set; } = [];

    [JsonPropertyName("header")]
    public DwHeader Header { get; set; } = null!;
}