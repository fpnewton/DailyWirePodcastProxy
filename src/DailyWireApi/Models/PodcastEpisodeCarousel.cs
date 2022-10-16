using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class PodcastEpisodeCarousel : IModule
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public IList<PodcastEpisode> PodcastEpisodes { get; set; } = new List<PodcastEpisode>();
}