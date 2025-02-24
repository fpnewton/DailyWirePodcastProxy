using System.Xml.Linq;
using MediatR;

namespace PodcastProxy.Application.Queries.GetPodcastFeed;

public class GetPodcastFeedQuery : IRequest<XDocument>
{
    public string PodcastId { get; set; } = null!;
    public string FeedUrl { get; set; } = null!;
}