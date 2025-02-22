namespace DailyWire.Api.Models;

public class DwContinueWatching : IDwModule
{
    public string Typename => "ContinueWatching";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
}