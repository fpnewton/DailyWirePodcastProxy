using AutoMapper;
using DailyWire.Api.Models;
using PodcastProxy.Domain.Entities;
using Podcast = PodcastProxy.Domain.Entities.Podcast;

namespace PodcastProxy.Application.Mappings;

public class PodcastMappingProfile : Profile
{
    public PodcastMappingProfile()
    {
        CreateMap<DwGetPodcastRes, Podcast>();
        
        CreateMap<DwGetPodcastRes, Season>()
            .ForMember(d => d.SeasonId, o => o.Ignore())
            .ForMember(d => d.PodcastId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Slug, o => o.Ignore())
            .ForMember(d => d.Name, o => o.Ignore())
            .ForMember(d => d.Description, o => o.Ignore())
            .ForMember(d => d.Podcast, o => o.Ignore())
            .ForMember(d => d.Episodes, o => o.Ignore());
    }
}