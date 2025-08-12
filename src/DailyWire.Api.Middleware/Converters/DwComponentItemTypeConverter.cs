using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Converters;

public class DwComponentItemTypeConverter : JsonConverter<DwComponentItemType>
{
    public override DwComponentItemType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString();

        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new JsonException("ComponentItem type value is null or empty.");
        }

        if (Enum.TryParse<DwComponentItemType>(raw, ignoreCase: true, out var result))
        {
            return result;
        }

        throw new KeyNotFoundException($"Unknown DwComponentRenderType: '{raw}' falling back to 'Generic' - This might be a bug!!!");
    }

    public override void Write(Utf8JsonWriter writer, DwComponentItemType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Enum.GetName(typeof(DwComponentRenderType), value)
                                ?? throw new JsonException($"Invalid DwComponentItemType value: {value}"));
    }
}