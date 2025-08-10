using System.Net;
using System.Xml;
using FastEndpoints;
using Flurl;
using PodcastProxy.Api.Extensions;
using PodcastProxy.Application.Queries.Podcasts;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class GetPodcastFeedRequest
{
    public string PodcastId { get; set; } = string.Empty;
}

public class GetPodcastFeedEndpoint : Endpoint<GetPodcastFeedRequest>
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
        var feedUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
            .AppendPathSegment(HttpContext.Request.PathBase)
            .AppendPathSegment(HttpContext.Request.Path);

        const string streamUrlSlug = "{Slug}";

        var streamUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
            .AppendPathSegment(HttpContext.Request.PathBase)
            .AppendPathSegments("daily-wire", "podcasts", "episodes", streamUrlSlug, "streams", "audio");

        var document = await new GetPodcastFeedQuery
        {
            PodcastId = req.PodcastId,
            FeedUrl = HttpContext.Request.Query.Aggregate(feedUrl, (url, pair) => url.SetQueryParam(pair.Key, pair.Value)),
            StreamUrl = streamUrl,
            StreamUrlSlug = WebUtility.UrlEncode(streamUrlSlug)
        }.ExecuteAsync(ct);

        if (!document.IsSuccess)
        {
            await this.SendResult(document, ct);
            return;
        }

        var stream = new MemoryStream();
        var settings = new XmlWriterSettings { Async = true };
        var writer = XmlWriter.Create(stream, settings);

        await document.Value.WriteToAsync(writer, ct);

        writer.Close();
        stream.Seek(0, SeekOrigin.Begin);

        await Send.StreamAsync(stream, contentType: "application/xml", cancellation: ct);
    }
}