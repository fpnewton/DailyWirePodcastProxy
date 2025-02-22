namespace DailyWire.Api.Models;

public class DwShowPremiere : IDwModule
{
    public string Typename => "ShowPremiere";
    
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public string? PremiereImage { get; set; }
    public DwShow? Show { get; set; }
}