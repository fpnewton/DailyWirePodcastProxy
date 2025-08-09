using Newtonsoft.Json;

namespace DailyWire.Api.Models;

[Obsolete]
public class DwGetPodcastRes
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
    public IList<DwSeasonDetails> Seasons { get; set; } = new List<DwSeasonDetails>();
    public DwUser? Author { get; set; }
}