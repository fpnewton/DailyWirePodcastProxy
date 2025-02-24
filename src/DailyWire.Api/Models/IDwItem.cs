namespace DailyWire.Api.Models;

public interface IDwItem
{
    public string Typename { get; }
    public string Id { get; set; }
}