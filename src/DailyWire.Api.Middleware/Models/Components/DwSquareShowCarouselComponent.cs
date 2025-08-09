using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Components;

public class DwSquareShowCarouselComponent : DwComponent
{
    public override DwComponentRenderType RenderType => DwComponentRenderType.SquareShowCarousel;

    [JsonPropertyName("items")]
    public List<DwItem> Items { get; set; } = [];

    [JsonPropertyName("header")]
    public DwHeader Header { get; set; } = null!;
}