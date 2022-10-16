using Microsoft.AspNetCore.Mvc;

namespace DailyWirePodcastProxy.Requests.Podcasts;

public class GetPodcastRequest
{
    [FromRoute]
    public string Slug { get; set; } = null!;
}