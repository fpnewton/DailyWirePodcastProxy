using System.Net;
using System.Xml;
using Ardalis.Result;
using FastEndpoints;
using Flurl;
using Microsoft.Extensions.Configuration;
using PodcastProxy.Api.Extensions;
using PodcastProxy.Application.Commands.Podcasts;
using PodcastProxy.Application.Queries.Podcasts;
using PodcastProxy.Application.Queries.Shows;

namespace PodcastProxy.Api.Endpoints.Podcasts;

public class GetPodcastFeedRequest
{
    public required string PodcastId { get; set; }
}

public class GetPodcastFeedEndpoint(IConfiguration configuration) : Endpoint<GetPodcastFeedRequest>
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
        var podcast = await new GetPodcastByIdQuery { PodcastId = req.PodcastId }.ExecuteAsync(ct);

        if (!podcast.IsSuccess)
        {
            var shows = await new GetShowsQuery().ExecuteAsync(ct);

            if (!shows.IsSuccess)
            {
                await this.SendResult(shows.Map(), ct);
                return;
            }

            var show = shows.Value.FirstOrDefault(s => string.Equals(s.Show.Id, req.PodcastId, StringComparison.Ordinal));

            if (show is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            podcast = await new EnsurePodcastExistsCommand { PodcastSlug = show.Show.Slug }.ExecuteAsync(ct);

            if (!podcast.IsSuccess)
            {
                await this.SendResult(shows.Map(), ct);
                return;
            }
        }

        var feedUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
            .AppendPathSegment(HttpContext.Request.PathBase)
            .AppendPathSegment(HttpContext.Request.Path);

        const string streamUrlSlug = "{Slug}";

        var streamUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
            .AppendPathSegment(HttpContext.Request.PathBase)
            .AppendPathSegments("daily-wire", "podcasts", "episodes", streamUrlSlug, "streams", "audio")
            .SetQueryParam("auth", configuration["Authentication:AccessKey"]);

        var document = await new GetPodcastFeedQuery
        {
            PodcastSlug = podcast.Value.Slug,
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