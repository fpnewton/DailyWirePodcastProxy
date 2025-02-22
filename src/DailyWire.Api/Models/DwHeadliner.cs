using DailyWire.Api.Converters;
using Newtonsoft.Json;

namespace DailyWire.Api.Models;

public class DwHeadliner : IDwModule
{
    public string Typename => "Headliner";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceSlug { get; set; }
    public string? Routing { get; set; }
    
    [JsonProperty("CTAText")]
    public string? CtaText { get; set; }
    
    public string? Link { get; set; }
    
    [JsonConverter(typeof(ItemConverter))]
    public IDwItem? Item { get; set; }
}