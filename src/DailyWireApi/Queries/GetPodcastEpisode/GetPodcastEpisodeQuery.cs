using DailyWireApi.Models;
using MediatR;

namespace DailyWireApi.Queries.GetPodcastEpisode;

public class GetPodcastEpisodeQuery : IRequest<GetPodcastEpisodeRes>
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
}