using DailyWireApi.Converters;
using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class ModularPageRes
{
    public string Id { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public bool? IsPublished { get; set; }
    public int? NextOrder { get; set; }
    
    [JsonConverter(typeof(ModuleListConverter))]
    public IList<IModule> Modules { get; set; } = new List<IModule>();
    
    public DateTimeOffset? CreatedAt { get; set; }
    public User? CreatedBy { get; set; }
}