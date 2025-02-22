namespace PodcastProxy.Domain.Services;

public interface IAuthenticationDetailsProvider
{
    public bool AccessKeyRequirementEnabled();
    public string? GetApiAccessKey();
    public string? CreateApiAccessKey();
}