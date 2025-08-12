using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Converters;

namespace DailyWire.Api.Middleware.Enums;

[JsonConverter(typeof(DwStatusConverter))]
public enum DwStatus
{
    Unknown,
    Published,
    Scheduled
};