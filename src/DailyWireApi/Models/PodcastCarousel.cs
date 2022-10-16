using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class PodcastCarousel : IModule
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public IList<Podcast> Podcasts { get; set; } = new List<Podcast>();
}