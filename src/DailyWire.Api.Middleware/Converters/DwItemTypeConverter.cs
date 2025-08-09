using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Converters;

public class DwItemTypeConverter : JsonConverter<DwItemType>
{
    private static readonly Dictionary<string, DwItemType> ReadMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Clip"] = DwItemType.Clip,
        ["Host"] = DwItemType.Host,
        ["ExternalLink"] = DwItemType.ExternalLink,
        ["Post"] = DwItemType.Post,
        ["Show"] = DwItemType.Show,
        ["ShowEpisode"] = DwItemType.ShowEpisode,
        ["Video"] = DwItemType.Video
    };

    private static readonly Dictionary<DwItemType, string> WriteMap = new()
    {
        [DwItemType.Clip] = "Clip",
        [DwItemType.ExternalLink] = "ExternalLink",
        [DwItemType.Post] = "Post",
        [DwItemType.Show] = "Show",
        [DwItemType.ShowEpisode] = "ShowEpisode",
        [DwItemType.Video] = "Video"
        // DwItemType.Unknown intentionally not mapped
    };

    public override DwItemType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Prefer strings like "Clip", but allow numeric enums too
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();

            if (!string.IsNullOrEmpty(s) && ReadMap.TryGetValue(s, out var t))
            {
                return t;
            }

            return DwItemType.Unknown;
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var i))
        {
            // Accept numeric value if defined; otherwise Unknown
            return Enum.IsDefined(typeof(DwItemType), i) ? (DwItemType)i : DwItemType.Unknown;
        }

        return DwItemType.Unknown;
    }

    public override void Write(Utf8JsonWriter writer, DwItemType value, JsonSerializerOptions options)
    {
        if (WriteMap.TryGetValue(value, out var s))
        {
            writer.WriteStringValue(s);
        }
        else
        {
            // Keep your existing behavior: write null for Unknown/unmapped
            writer.WriteNullValue();
        }
    }
}