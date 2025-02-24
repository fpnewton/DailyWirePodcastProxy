using AutoMapper;
using DailyWire.Api.Models;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Mappings;

public class SeasonMappingProfile : Profile
{
    public SeasonMappingProfile()
    {
        CreateMap<DwSeasonDetails, Season>()
            .ForMember(d => d.SeasonId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.PodcastId, o => o.Ignore())
            .ForMember(d => d.Podcast, o => o.Ignore())
            .ForMember(d => d.Episodes, o => o.Ignore());
    }
}