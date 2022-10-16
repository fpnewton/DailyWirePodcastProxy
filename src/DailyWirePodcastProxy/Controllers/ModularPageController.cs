using DailyWireApi.Models;
using DailyWireApi.Queries.GetModularPage;
using DailyWireApi.Queries.GetModularPageSlugs;
using DailyWirePodcastProxy.Attributes;
using DailyWirePodcastProxy.Requests.ModularPages;
using Microsoft.AspNetCore.Mvc;

namespace DailyWirePodcastProxy.Controllers;

[ApiController]
[Route("modular-pages")]
public class ModularPageController : BaseController
{
    /// <summary>
    /// Gets a list of modular pages.
    /// A modular page is a dynamic list of content, such as the podcasts page within the mobile app.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IList<ModulePage>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModularPageSlugs([BindRequest] GetModularPageSlugsRequest request, CancellationToken cancellationToken) =>
        await InvokeMediatorRequest<GetModularPageSlugsQuery>(request, cancellationToken);

    /// <summary>
    /// Gets the contents of a modular page.
    /// </summary>
    [HttpGet("{Slug}")]
    [ProducesResponseType(typeof(ModularPageRes), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModularPage([BindRequest] GetModularPageRequest request, CancellationToken cancellationToken) =>
        await InvokeMediatorRequest<GetModularPageQuery>(request, cancellationToken);
}