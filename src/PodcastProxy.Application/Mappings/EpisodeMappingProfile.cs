using AutoMapper;
using DailyWire.Api.Models;
using Episode = PodcastProxy.Domain.Entities.Episode;

namespace PodcastProxy.Application.Mappings;

public class EpisodeMappingProfile : Profile
{
    public EpisodeMappingProfile()
    {
        CreateMap<DwGetPodcastEpisodeRes, Episode>()
            .ForMember(d => d.EpisodeId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.SeasonId, o => o.Ignore())
            .ForMember(d => d.Season, o => o.Ignore());
    }
}