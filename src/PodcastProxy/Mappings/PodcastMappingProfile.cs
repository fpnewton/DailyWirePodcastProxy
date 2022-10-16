using AutoMapper;
using DailyWireApi.Models;
using PodcastDatabase.Entities;
using Podcast = PodcastDatabase.Entities.Podcast;

namespace PodcastProxy.Mappings;

public class PodcastMappingProfile : Profile
{
    public PodcastMappingProfile()
    {
        CreateMap<GetPodcastRes, Podcast>();
        
        CreateMap<GetPodcastRes, Season>()
            .ForMember(d => d.SeasonId, o => o.Ignore())
            .ForMember(d => d.PodcastId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Slug, o => o.Ignore())
            .ForMember(d => d.Name, o => o.Ignore())
            .ForMember(d => d.Description, o => o.Ignore())
            .ForMember(d => d.Podcast, o => o.Ignore())
            .ForMember(d => d.Episodes, o => o.Ignore());
    }
}