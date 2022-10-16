using MediatR;

namespace PodcastProxy.Commands.CheckPodcastNewEpisodes;

public class CheckPodcastNewEpisodesCommand : IRequest
{
    public string PodcastId { get; set; } = null!;
}