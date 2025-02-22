using System.Xml;
using System.Xml.Linq;
using FastEndpoints;
using Flurl;
using MediatR;
using PodcastProxy.Application.Queries.GetPodcastFeed;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class GetPodcastFeedRequest
{
    public string PodcastId { get; set; } = string.Empty;
}

public class GetPodcastFeedEndpoint(IMediator mediator) : Endpoint<GetPodcastFeedRequest>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif

        Get("podcasts/{PodcastId}/feed");
    }

    public override async Task HandleAsync(GetPodcastFeedRequest req, CancellationToken ct)
    {
        var document = await GetPodcastFeed(req, ct);
        var stream = new MemoryStream();
        var settings = new XmlWriterSettings { Async = true };
        var writer = XmlWriter.Create(stream, settings);

        await document.WriteToAsync(writer, ct);

        writer.Close();
        stream.Seek(0, SeekOrigin.Begin);

        await SendStreamAsync(stream, contentType: "application/xml", cancellation: ct);
    }

    private async Task<XDocument> GetPodcastFeed(GetPodcastFeedRequest request, CancellationToken cancellationToken)
    {
        var feedUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
            .AppendPathSegment(HttpContext.Request.PathBase)
            .AppendPathSegment(HttpContext.Request.Path);

        var query = new GetPodcastFeedQuery
        {
            PodcastId = request.PodcastId,
            FeedUrl = HttpContext.Request.Query.Aggregate(feedUrl, (url, pair) => url.SetQueryParam(pair.Key, pair.Value))
        };

        return await mediator.Send(query, cancellationToken);
    }
}