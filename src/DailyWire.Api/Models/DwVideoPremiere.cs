namespace DailyWire.Api.Models;

public class DwVideoPremiere : IDwModule
{
    public string Typename => "VideoPremiere";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? PremiereImage { get; set; }
    public DwVideo? Video { get; set; }
}