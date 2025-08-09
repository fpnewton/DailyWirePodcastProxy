using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwPaginatedPage
{
    [JsonPropertyName("componentItems")]
    public List<DwComponentItem> ComponentItems { get; set; } = [];
    
    [JsonPropertyName("nextPageUrl")]
    public string? NextPageUrl { get; set; }
}