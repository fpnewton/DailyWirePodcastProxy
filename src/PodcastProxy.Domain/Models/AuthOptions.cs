namespace PodcastProxy.Domain.Models;

public class AuthOptions
{
    public const string ConfigSectionName = "Authentication";

    public bool Enabled { get; set; } = true;
    public string AccessKey { get; set; } = string.Empty;
}