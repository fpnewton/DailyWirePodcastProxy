using Newtonsoft.Json;

namespace DailyWireApi.Models;

public class VideoPremiere : IModule
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? PremiereImage { get; set; }
    public Video? Video { get; set; }
}