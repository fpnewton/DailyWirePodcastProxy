using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Items;

public class DwShowItem : DwItem
{
    [JsonPropertyName("type")]
    public override DwItemType Type => DwItemType.Show;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("show")]
    public DwShowDetails Show { get; set; } = null!;
}