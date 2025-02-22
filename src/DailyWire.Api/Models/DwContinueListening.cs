namespace DailyWire.Api.Models;

public class DwContinueListening : IDwModule
{
    public string Typename => "ContinueListening";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? Title { get; set; }
}