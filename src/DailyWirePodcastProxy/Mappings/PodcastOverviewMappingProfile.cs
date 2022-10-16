using AutoMapper;
using DailyWireApi.Models;
using DailyWirePodcastProxy.Models;

namespace DailyWirePodcastProxy.Mappings;

public class PodcastOverviewMappingProfile : Profile
{
    public PodcastOverviewMappingProfile()
    {
        CreateMap<Podcast, PodcastOverview>()
            .ForMember(d => d.Feed, o => o.MapFrom<PodcastFeedValueResolver>());
    }
}