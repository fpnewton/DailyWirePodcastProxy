using DailyWireApi.Models;

namespace DailyWireApi.Queries.ListPodcastEpisode;

public class ListPodcastEpisodeQueryResponse
{
    public IList<GetPodcastEpisodeRes>? ListPodcastEpisode { get; set; }
}