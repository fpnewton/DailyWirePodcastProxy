using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Models.Items;

public class DwPostItem : DwItem
{
    [JsonPropertyName("type")]
    public override DwItemType Type => DwItemType.Post;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("post")]
    public DwPost Post { get; set; } = null!;
}