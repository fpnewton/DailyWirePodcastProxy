using Newtonsoft.Json;

namespace DailyWireApi.Models;

public interface IModule
{
    [JsonProperty("__typename")]
    public string Typename { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
}