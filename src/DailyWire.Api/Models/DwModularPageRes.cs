using DailyWire.Api.Converters;
using Newtonsoft.Json;

namespace DailyWire.Api.Models;

[Obsolete]
public class DwModularPageRes
{
    public string Id { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public bool? IsPublished { get; set; }
    public int? NextOrder { get; set; }
    
    [JsonConverter(typeof(ModuleListConverter))]
    public IList<IDwModule> Modules { get; set; } = new List<IDwModule>();
    
    public DateTimeOffset? CreatedAt { get; set; }
    public DwUser? CreatedBy { get; set; }
}