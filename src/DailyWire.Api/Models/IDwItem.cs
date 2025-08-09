namespace DailyWire.Api.Models;

[Obsolete]
public interface IDwItem
{
    public string Typename { get; }
    public string Id { get; set; }
}