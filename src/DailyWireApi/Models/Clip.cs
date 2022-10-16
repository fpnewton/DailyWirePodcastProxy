using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class Clip : IItem
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    public string Id { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Image { get; set; }
    public Video? Video { get; set; }
}