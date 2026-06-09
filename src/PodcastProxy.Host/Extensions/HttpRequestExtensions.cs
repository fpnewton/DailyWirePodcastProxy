using Microsoft.AspNetCore.Http;

namespace PodcastProxy.Host.Extensions;

public static class HttpRequestExtensions
{
    private const string RedactedValue = "[REDACTED]";

    public static string ToRequestLogLine(this HttpRequest request)
    {
        var query = request.Query.Aggregate(string.Empty, (str, pair) =>
        {
            var separator = string.IsNullOrEmpty(str) ? '?' : '&';

            var value = string.Equals(pair.Key, "auth", StringComparison.OrdinalIgnoreCase)
                ? RedactedValue
                : pair.Value.ToString();

            var sanitizedValue = value
                .Replace(Environment.NewLine, "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
            
            return str + $"{separator}{pair.Key}={sanitizedValue}";
        });

        return $"{request.Method} {request.Scheme}://{request.Host}{request.Path}{query} {request.Protocol}";
    }
}
