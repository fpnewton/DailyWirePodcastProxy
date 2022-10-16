namespace DailyWireApi.Models;

public class User
{
    public string Id { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
}