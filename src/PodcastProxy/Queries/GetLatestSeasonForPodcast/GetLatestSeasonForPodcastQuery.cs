using MediatR;
using PodcastDatabase.Entities;

namespace PodcastProxy.Queries.GetLatestSeasonForPodcast;

public class GetLatestSeasonForPodcastQuery : IRequest<Season?>
{
    public string PodcastId { get; set; } = null!;
}