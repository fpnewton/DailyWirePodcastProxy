using AutoMapper;
using DailyWireApi.Models;
using PodcastDatabase.Entities;

namespace PodcastProxy.Mappings;

public class SeasonMappingProfile : Profile
{
    public SeasonMappingProfile()
    {
        CreateMap<SeasonDetails, Season>()
            .ForMember(d => d.SeasonId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.PodcastId, o => o.Ignore())
            .ForMember(d => d.Podcast, o => o.Ignore())
            .ForMember(d => d.Episodes, o => o.Ignore());
    }
}