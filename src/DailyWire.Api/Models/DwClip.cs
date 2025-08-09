namespace DailyWire.Api.Models;

[Obsolete]
public class DwClip : IDwItem
{
    public string Typename => "Clip";
    
    public string Id { get; set; } = null!;
    
    public string? Title { get; set; }
    public string? Image { get; set; }
    public DwVideo? Video { get; set; }
}