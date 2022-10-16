using System.Xml;
using DailyWireApi.Models;
using DailyWireApi.Queries.GetModularPage;
using DailyWireApi.Queries.GetPodcast;
using DailyWireApi.Queries.ListPodcastEpisode;
using DailyWirePodcastProxy.Attributes;
using DailyWirePodcastProxy.Models;
using DailyWirePodcastProxy.Requests.Podcasts;
using Microsoft.AspNetCore.Mvc;
using PodcastProxy.Queries.GetPodcastFeed;

namespace DailyWirePodcastProxy.Controllers;

[ApiController]
[Route("podcasts")]
public class PodcastsController : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(ICollection<PodcastOverview>), StatusCodes.Status200OK)]
    public async Task<ICollection<PodcastOverview>> GetPodcasts(CancellationToken cancellationToken)
    {
        var query = new GetModularPageQuery { Slug = "listen" };
        var listenPageRes = await Mediator.Send(query, cancellationToken);
        var results = new List<PodcastOverview>();

        foreach (var module in listenPageRes.Modules)
        {
            if (module is PodcastCarousel carousel)
            {
                var podcasts = Mapper.Map<ICollection<PodcastOverview>>(carousel.Podcasts);

                results.AddRange(podcasts);
            }
        }

        return results;
    }

    [HttpGet("{Slug}")]
    [ProducesResponseType(typeof(GetPodcastRes), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPodcast([BindRequest] GetPodcastRequest request, CancellationToken cancellationToken) =>
        await InvokeMediatorRequest<GetPodcastQuery>(request, cancellationToken);

    [HttpGet("seasons/{Slug}/episodes")]
    [ProducesResponseType(typeof(IList<GetPodcastEpisodeRes>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPodcastEpisodes([BindRequest] ListPodcastEpisodesRequest request, CancellationToken cancellationToken) =>
        await InvokeMediatorRequest<ListPodcastEpisodeQuery>(request, cancellationToken);

    [HttpGet("{PodcastId}/feed")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPodcastFeed([FromRoute] GetPodcastFeedQuery query, CancellationToken cancellationToken)
    {
        var document = await Mediator.Send(query, cancellationToken);
        var stream = new MemoryStream();
        var settings = new XmlWriterSettings { Async = true };
        var writer = XmlWriter.Create(stream, settings);

        await document.WriteToAsync(writer, cancellationToken);

        writer.Close();
        stream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(stream, "application/xml");
    }
}