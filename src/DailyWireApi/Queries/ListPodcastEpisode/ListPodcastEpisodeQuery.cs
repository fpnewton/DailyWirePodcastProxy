using DailyWireApi.Models;
using MediatR;

namespace DailyWireApi.Queries.ListPodcastEpisode;

public class ListPodcastEpisodeQuery : IRequest<IList<GetPodcastEpisodeRes>>
{
    public string? SeasonId { get; set; }
    public int? First { get; set; }
    public int? Skip { get; set; }
}