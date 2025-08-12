using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.Items;

namespace DailyWire.Api.Middleware.Converters;

public class DwItemConverter : JsonConverter<DwItem>
{
    private static readonly Dictionary<DwItemType, Type> TypeMap = new()
    {
        { DwItemType.Clip, typeof(DwClipItem) },
        { DwItemType.Host, typeof(DwHostItem) },
        { DwItemType.ExternalLink, typeof(DwExternalLinkItem) },
        { DwItemType.Post, typeof(DwPostItem) },
        { DwItemType.Show, typeof(DwShowItem) },
        { DwItemType.ShowEpisode, typeof(DwShowEpisodeItem) },
        { DwItemType.Video, typeof(DwVideoItem) }
    };

    public override DwItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
        {
            throw new JsonException("Missing discriminator property 'type'.");
        }

        var rawTypeJson = typeProp.GetRawText();

        var rawTypeString = typeProp.ValueKind switch
        {
            JsonValueKind.String => typeProp.GetString(),
            JsonValueKind.Number => typeProp.GetRawText(), // number as text
            _ => typeProp.GetRawText()
        };

        var dwType = JsonSerializer.Deserialize<DwItemType>(rawTypeJson, options);

        if (dwType == DwItemType.Unknown)
        {
            Console.Error.WriteLine($"Unknown DwItemType value in JSON: {rawTypeString}");

            return null;
        }

        if (!TypeMap.TryGetValue(dwType, out var target))
        {
            throw new JsonException($"No mapping registered for DwItemType '{dwType}'.");
        }

        // Deserialize the whole object into the chosen type
        var json = root.GetRawText();

        return (DwItem?)JsonSerializer.Deserialize(json, target, options);
    }

    public override void Write(Utf8JsonWriter writer, DwItem value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}