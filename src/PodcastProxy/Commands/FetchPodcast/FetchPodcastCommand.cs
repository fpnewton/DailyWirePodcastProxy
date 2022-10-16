using MediatR;
using PodcastDatabase.Entities;

namespace PodcastProxy.Commands.FetchPodcast;

public class FetchPodcastCommand : IRequest<Podcast>
{
    public string PodcastId { get; set; } = null!;
}