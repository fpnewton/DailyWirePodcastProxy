using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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

    [Fact]
    public void ToRequestLogLine_RemovesNewlinesFromQueryValues()
    {
        var context = CreateContext();
        context.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            ["format"] = "json\r\nforged-entry"
        });

        var logLine = context.Request.ToRequestLogLine();

        Assert.Contains("format=jsonforged-entry", logLine);
        Assert.DoesNotContain('\r', logLine);
        Assert.DoesNotContain('\n', logLine);
    }

    [Fact]
    public void ToRequestLogLine_RemovesNewlinesFromQueryParameterKeys()
    {
        var context = CreateContext();
        context.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            ["for\r\nged"] = "value"
        });

        var logLine = context.Request.ToRequestLogLine();

        Assert.Contains("forged=value", logLine);
        Assert.DoesNotContain('\r', logLine);
        Assert.DoesNotContain('\n', logLine);
    }

    [Fact]
    public void ToRequestLogLine_SanitizesSuspiciousRequestContent()
    {
        var context = CreateContext();
        context.Request.Method = "GE\r\nT";
        context.Request.Scheme = "ht\ntps";
        context.Features.Get<IHttpRequestFeature>()!.Headers["Host"] = "example.com\r\nforged-host";
        context.Request.Path = new PathString("/podcasts\r\nforged-path");
        context.Request.Protocol = "HTTP/1.1\r\nforged-protocol";
        context.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            ["filter\r\nname"] = "<value>&\"'\r\nforged-query"
        });

        var logLine = context.Request.ToRequestLogLine();

        Assert.DoesNotContain('\r', logLine);
        Assert.DoesNotContain('\n', logLine);
        Assert.Contains("example.comforged-host/podcastsforged-path", logLine);
        Assert.Contains("filtername=&lt;value&gt;&amp;&quot;&#39;forged-query", logLine);
        Assert.DoesNotContain("&amp;lt;", logLine);
    }

    [Fact]
    public void ToRequestLogLine_IncludesUsefulRequestInformation()
    {
        var context = CreateContext();
        context.Request.QueryString = new QueryString("?format=json&season=2");

        var logLine = context.Request.ToRequestLogLine();

        Assert.Equal("GET https://example.com/podcasts?format=json&season=2 HTTP/1.1", logLine);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/podcasts";
        context.Request.Protocol = "HTTP/1.1";
        return context;
    }
}
