using Newtonsoft.Json;

namespace DailyWireApi.Models;

public interface IItem
{
    [JsonProperty("__typename")]
    public string Typename { get; set; }
    public string Id { get; set; }
}