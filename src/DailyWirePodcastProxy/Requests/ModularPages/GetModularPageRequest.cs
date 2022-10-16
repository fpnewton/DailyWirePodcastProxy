using Microsoft.AspNetCore.Mvc;

namespace DailyWirePodcastProxy.Requests.ModularPages;

public class GetModularPageRequest
{
    /// <summary>
    /// Modular page slug.
    /// </summary>
    [FromRoute]
    public string Slug { get; set; } = string.Empty;
}