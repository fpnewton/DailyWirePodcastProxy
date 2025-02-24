using MediatR;

namespace PodcastProxy.Application.Commands.CheckPodcastNewEpisodes;

public class CheckPodcastNewEpisodesCommand : IRequest
{
    public string PodcastId { get; set; } = null!;
}