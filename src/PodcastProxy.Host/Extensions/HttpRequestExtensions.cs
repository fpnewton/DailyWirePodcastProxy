using Microsoft.AspNetCore.Http;

namespace PodcastProxy.Host.Extensions;

public static class HttpRequestExtensions
{
    public static string ToRequestLogLine(this HttpRequest request)
    {
        var query = request.Query.Aggregate(string.Empty, (str, pair) =>
        {
            var separator = string.IsNullOrEmpty(str) ? '?' : '&';
            
            var sanitizedValue = pair.Value.ToString()
                .Replace(Environment.NewLine, "")
                .Replace("\n", "")
                .Replace("\r", "");
            
            return str + $"{separator}{pair.Key}={sanitizedValue}";
        });

        return $"{request.Method} {request.Scheme}://{request.Host}{request.Path}{query} {request.Protocol}";
    }
}