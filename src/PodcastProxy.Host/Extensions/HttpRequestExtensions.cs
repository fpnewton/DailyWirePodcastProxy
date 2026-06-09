using Microsoft.AspNetCore.Http;
using System.Text;

namespace PodcastProxy.Host.Extensions;

public static class HttpRequestExtensions
{
    private const string RedactedValue = "[REDACTED]";

    public static string ToRequestLogLine(this HttpRequest request)
    {
        var query = new StringBuilder();

        foreach (var pair in request.Query)
        {
            query.Append(query.Length == 0 ? '?' : '&');
            query.Append(SanitizeForLog(pair.Key));
            query.Append('=');

            var value = string.Equals(pair.Key, "auth", StringComparison.OrdinalIgnoreCase)
                ? RedactedValue
                : pair.Value.ToString();

            query.Append(SanitizeForLog(value));
        }

        return $"{SanitizeForLog(request.Method)} " +
               $"{SanitizeForLog(request.Scheme)}://" +
               $"{SanitizeForLog(request.Host.Value)}" +
               $"{SanitizeForLog(request.Path.Value)}" +
               $"{query} " +
               $"{SanitizeForLog(request.Protocol)}";
    }

    private static string SanitizeForLog(string? value)
    {
        return (value ?? string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}
