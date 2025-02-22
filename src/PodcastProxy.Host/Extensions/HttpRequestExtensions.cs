using Microsoft.AspNetCore.Http;

namespace PodcastProxy.Host.Extensions;

public static class HttpRequestExtensions
{
    public static string ToRequestLogLine(this HttpRequest request)
    {
        var query = request.Query.Aggregate(string.Empty, (str, pair) =>
        {
            var separator = string.IsNullOrEmpty(str) ? '?' : '&';

            return str + $"{separator}{pair.Key}={pair.Value}";
        });

        return $"{request.Method} {request.Scheme}://{request.Host}{request.Path}{query} {request.Protocol}";
    }
}