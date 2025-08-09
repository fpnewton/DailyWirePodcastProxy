using System.Net.Http.Headers;
using DailyWire.Api.Streaming.Configuration;

namespace DailyWire.Api.Streaming.Services;

public interface IDailyWireStreamApi
{
    public Task<HttpResponseMessage> GetAudioStream(string audioStreamUrl, CancellationToken cancellationToken);
    public Task<HttpResponseMessage> GetAudioStream(string audioStreamUrl, Action<HttpRequestHeaders>? configureHeaders, CancellationToken cancellationToken);
}

public class DailyWireStreamApi(
    IHttpClientFactory httpClientFactory,
    DwStreamingConfiguration configuration
) : IDailyWireStreamApi
{
    public Task<HttpResponseMessage> GetAudioStream(string audioStreamUrl, CancellationToken cancellationToken) =>
        GetAudioStream(audioStreamUrl, null, cancellationToken);

    public async Task<HttpResponseMessage> GetAudioStream(string audioStreamUrl, Action<HttpRequestHeaders>? configureHeaders,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(DwStreamingConstants.HttpClientStreamProxy);
        var requestUrl = audioStreamUrl.Replace(configuration.BaseUrl, string.Empty);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        configureHeaders?.Invoke(request.Headers);

        return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }
}