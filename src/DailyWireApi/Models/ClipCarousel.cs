using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class ClipCarousel : IModule
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Layout { get; set; }
    public IList<Clip> Clips { get; set; } = new List<Clip>();
}