using AutoMapper;
using DailyWireApi.Queries.GetModularPage;
using DailyWireApi.Queries.GetModularPageSlugs;
using DailyWireApi.Queries.GetPodcast;
using DailyWireApi.Queries.ListPodcastEpisode;
using DailyWirePodcastProxy.Requests.ModularPages;
using DailyWirePodcastProxy.Requests.Podcasts;

namespace DailyWirePodcastProxy.Requests;

public class RequestMappingProfile : Profile
{
    public RequestMappingProfile()
    {
        CreateMap<GetModularPageRequest, GetModularPageQuery>();

        CreateMap<GetModularPageSlugsRequest, GetModularPageSlugsQuery>();

        CreateMap<GetPodcastRequest, GetPodcastQuery>()
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<ListPodcastEpisodesRequest, ListPodcastEpisodeQuery>()
            .ForMember(d => d.SeasonId, o => o.Ignore());
    }
}