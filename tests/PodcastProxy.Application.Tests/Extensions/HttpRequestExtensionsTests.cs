using Microsoft.AspNetCore.Http;
using PodcastProxy.Host.Extensions;

namespace PodcastProxy.Application.Tests.Extensions;

public class HttpRequestExtensionsTests
{
    [Theory]
    [InlineData("auth")]
    [InlineData("AUTH")]
    public void ToRequestLogLine_RedactsAuthQueryParameter(string parameterName)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/podcasts";
        context.Request.QueryString = new QueryString($"?{parameterName}=secret-key&format=json");

        var logLine = context.Request.ToRequestLogLine();

        Assert.Contains($"{parameterName}=[REDACTED]", logLine);
        Assert.Contains("format=json", logLine);
        Assert.DoesNotContain("secret-key", logLine);
    }
}
