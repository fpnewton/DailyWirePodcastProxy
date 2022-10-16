using Microsoft.AspNetCore.Mvc;

namespace DailyWirePodcastProxy.Requests.ModularPages;

public class GetModularPageSlugsRequest
{
    [FromQuery]
    public int First { get; set; } = 10;

    [FromQuery]
    public int Skip { get; set; } = 0;
}