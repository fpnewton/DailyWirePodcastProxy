using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Converters;

public class DwSortOrderDirectionConverter : JsonConverter<DwSortOrderDirection>
{
    private static readonly Dictionary<string, DwSortOrderDirection> ReadMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ASC"] = DwSortOrderDirection.Ascending,
        ["DESC"] = DwSortOrderDirection.Descending
    };

    private static readonly Dictionary<DwSortOrderDirection, string> WriteMap = new()
    {
        [DwSortOrderDirection.Ascending] = "ASC",
        [DwSortOrderDirection.Descending] = "DESC"
    };

    public override DwSortOrderDirection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Prefer strings like "Clip", but allow numeric enums too
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();

            if (!string.IsNullOrEmpty(s) && ReadMap.TryGetValue(s, out var t))
            {
                return t;
            }

            return DwSortOrderDirection.Descending;
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var i))
        {
            // Accept numeric value if defined; otherwise Unknown
            return Enum.IsDefined(typeof(DwSortOrderDirection), i) ? (DwSortOrderDirection)i : DwSortOrderDirection.Descending;
        }

        return DwSortOrderDirection.Descending;
    }

    public override void Write(Utf8JsonWriter writer, DwSortOrderDirection value, JsonSerializerOptions options)
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