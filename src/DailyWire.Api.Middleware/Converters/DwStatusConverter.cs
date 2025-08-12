using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;

namespace DailyWire.Api.Middleware.Converters;

public class DwStatusConverter : JsonConverter<DwStatus>
{
    public override bool CanConvert(Type t) => t == typeof(DwStatus);

    public override DwStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "PUBLISHED" => DwStatus.Published,
            "SCHEDULED" => DwStatus.Scheduled,
            _ => DwStatus.Unknown
        };

    public override void Write(Utf8JsonWriter writer, DwStatus value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case DwStatus.Published:
                JsonSerializer.Serialize(writer, "PUBLISHED", options);
                return;

            case DwStatus.Scheduled:
                JsonSerializer.Serialize(writer, "SCHEDULED", options);
                return;

            case DwStatus.Unknown:
                JsonSerializer.Serialize(writer, null!, options);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }

        throw new Exception("Cannot marshal type Status");
    }
}