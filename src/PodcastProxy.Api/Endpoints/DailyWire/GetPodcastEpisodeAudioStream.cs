using System.Net;
using System.Net.Http.Headers;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Services;
using DailyWire.Api.Streaming.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPodcastEpisodeAudioStreamRequest
{
    public required string Slug { get; set; }
}

public class GetPodcastEpisodeAudioStreamEndpoint(
    IDailyWireMiddlewareApi dwApiService,
    IDailyWireStreamApi dwStreamService
) : Endpoint<GetPodcastEpisodeAudioStreamRequest>
{
    public override void Configure()
    {
        Get("daily-wire/podcasts/episodes/{Slug}/streams/audio");
    }

    public override async Task HandleAsync(GetPodcastEpisodeAudioStreamRequest req, CancellationToken ct)
    {
        var episode = await dwApiService.GetEpisode(req.Slug, ct);

        if (!episode.IsSuccess)
        {
            await this.SendResult(episode, ct);
            return;
        }

        if (episode.Value.Status != DwStatus.Published)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (string.IsNullOrEmpty(episode.Value.AudioUrl))
        {
            AddError("Episode audio url is not valid.");

            await Send.ErrorsAsync(StatusCodes.Status422UnprocessableEntity, ct);
            return;
        }

        using var upstream = await dwStreamService.GetMediaStream(episode.Value.AudioUrl, headers =>
        {
            CopyHeaderIfPresent(HttpContext.Request.Headers, headers, "Range");
            CopyHeaderIfPresent(HttpContext.Request.Headers, headers, "If-None-Match");
            CopyHeaderIfPresent(HttpContext.Request.Headers, headers, "If-Modified-Since");
        }, ct);

        HttpContext.Response.StatusCode = (int)upstream.StatusCode;

        Copy(outgoing: HttpContext.Response.Headers, incoming: upstream.Headers);
        Copy(outgoing: HttpContext.Response.Headers, incoming: upstream.Content.Headers);

        HttpContext.Response.Headers["X-Accel-Buffering"] = "no";

        if (upstream.StatusCode != HttpStatusCode.NotModified)
        {
            await using var stream = await upstream.Content.ReadAsStreamAsync(ct);
            await stream.CopyToAsync(HttpContext.Response.Body, ct);
        }
    }

    private static void CopyHeaderIfPresent(IHeaderDictionary from, HttpRequestHeaders to, string name)
    {
        if (from.TryGetValue(name, out StringValues values) && values.Count > 0)
        {
            to.TryAddWithoutValidation(name, values.ToArray());
        }
    }

    private static void Copy(IHeaderDictionary outgoing, HttpHeaders incoming)
    {
        foreach (var (key, vals) in incoming)
        {
            outgoing[key] = vals.ToArray();
        }
    }
}