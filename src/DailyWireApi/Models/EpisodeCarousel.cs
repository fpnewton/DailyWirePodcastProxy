using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class EpisodeCarousel : IModule
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;

    public string? Title { get; set; }
    public int? Order { get; set; }
    public IList<Episode> Episodes { get; set; } = new List<Episode>();
}