using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Converters;

namespace DailyWire.Api.Middleware.Enums;

[JsonConverter(typeof(DwSortOrderDirectionConverter))]
public enum DwSortOrderDirection
{
    Ascending,
    Descending
}