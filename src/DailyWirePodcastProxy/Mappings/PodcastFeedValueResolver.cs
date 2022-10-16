using AutoMapper;
using DailyWireApi.Models;
using DailyWirePodcastProxy.Models;
using Flurl;

namespace DailyWirePodcastProxy.Mappings;

public class PodcastFeedValueResolver : IValueResolver<Podcast, PodcastOverview, Uri>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public PodcastFeedValueResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public Uri Resolve(Podcast source, PodcastOverview destination, Uri destMember, ResolutionContext context)
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return null!;
        }

        var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
        var host = _httpContextAccessor.HttpContext.Request.Host;

        return new Url($"{scheme}://{host}")
            .AppendPathSegment(_httpContextAccessor.HttpContext.Request.PathBase)
            .AppendPathSegment(_httpContextAccessor.HttpContext.Request.Path)
            .AppendPathSegments(source.Id, "feed")
            .SetQueryParam("auth", _configuration["Authentication:AccessKey"])
            .ToUri();
    }
}