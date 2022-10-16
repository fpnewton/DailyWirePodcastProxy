using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class GetPodcastRes
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;

    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? CoverImage { get; set; }
    public string? BackgroundImage { get; set; }
    public string? LogoImage { get; set; }
    public string? BelongsTo { get; set; }
    public IList<SeasonDetails> Seasons { get; set; } = new List<SeasonDetails>();
    public User? Author { get; set; }
}