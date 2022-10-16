using DailyWireApi.Models;
using MediatR;

namespace DailyWireApi.Queries.GetPodcast;

public class GetPodcastQuery : IRequest<GetPodcastRes>
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
}