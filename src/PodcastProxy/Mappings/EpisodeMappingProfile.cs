using AutoMapper;
using DailyWireApi.Models;
using Episode = PodcastDatabase.Entities.Episode;

namespace PodcastProxy.Mappings;

public class EpisodeMappingProfile : Profile
{
    public EpisodeMappingProfile()
    {
        CreateMap<GetPodcastEpisodeRes, Episode>()
            .ForMember(d => d.EpisodeId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.SeasonId, o => o.Ignore())
            .ForMember(d => d.Season, o => o.Ignore());
    }
}