using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Converters;

public class DwComponentRenderTypeConverter : JsonConverter<DwComponentRenderType>
{
    public override DwComponentRenderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString();

        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new JsonException("renderType value is null or empty.");
        }

        if (Enum.TryParse<DwComponentRenderType>(raw, ignoreCase: true, out var result))
        {
            return result;
        }

        Console.Error.WriteLine($"Unknown DwComponentRenderType: '{raw}' falling back to 'Generic' - This might be a bug!!!");

        return DwComponentRenderType.Generic;
    }

    public override void Write(Utf8JsonWriter writer, DwComponentRenderType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Enum.GetName(typeof(DwComponentRenderType), value)
                                ?? throw new JsonException($"Invalid DwComponentRenderType value: {value}"));
    }
}