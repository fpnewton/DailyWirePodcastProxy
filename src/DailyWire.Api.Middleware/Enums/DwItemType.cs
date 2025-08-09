using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Converters;

namespace DailyWire.Api.Middleware.Enums;

[JsonConverter(typeof(DwItemTypeConverter))]
public enum DwItemType
{
    Clip,
    Host,
    ExternalLink,
    Post,
    Show,
    ShowEpisode,
    Video,
    Unknown
};