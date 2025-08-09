namespace DailyWire.Api.Models;

[Obsolete]
public interface IDwModule
{
    public string Typename { get; }
    public string Id { get; set; }
    public string Type { get; set; }
}