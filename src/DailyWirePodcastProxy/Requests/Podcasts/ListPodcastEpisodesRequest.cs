using Microsoft.AspNetCore.Mvc;

namespace DailyWirePodcastProxy.Requests.Podcasts;

public class ListPodcastEpisodesRequest
{
    [FromRoute]
    public string Slug { get; set; } = null!;
    
    [FromQuery]
    public int? First { get; set; }
    
    [FromQuery]
    public int? Skip { get; set; }
}