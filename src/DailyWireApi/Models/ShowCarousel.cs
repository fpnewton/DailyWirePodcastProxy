using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class ShowCarousel : IModule
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Orientation { get; set; }
    public IList<Show> Shows { get; set; } = new List<Show>();
}