using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.ComponentItems;

namespace DailyWire.Api.Middleware.Converters;

public class DwComponentItemConverter : JsonConverter<DwComponentItem>
{
    private static readonly Dictionary<DwComponentItemType, Type> TypeMap = new()
    {
        { DwComponentItemType.ShowEpisode, typeof(DwShowEpisodeComponentItem) }
    };

    public override DwComponentItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
        {
            throw new JsonException("Missing discriminator property 'type'.");
        }

        // Use our enum converter to parse renderType
        var renderType = JsonSerializer.Deserialize<DwComponentItemType>(typeProp.GetRawText(), options);

        if (!TypeMap.TryGetValue(renderType, out var targetType))
        {
            Console.Error.WriteLine($"No mapping registered for DwComponentItemType '{renderType}'.");
            return null;
        }

        return (DwComponentItem?)JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
    }

    public override void Write(Utf8JsonWriter writer, DwComponentItem value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}