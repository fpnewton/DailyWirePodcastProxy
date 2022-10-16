using System.Xml.Linq;
using MediatR;

namespace PodcastProxy.Queries.GetPodcastFeed;

public class GetPodcastFeedQuery : IRequest<XDocument>
{
    public string PodcastId { get; set; } = null!;
}