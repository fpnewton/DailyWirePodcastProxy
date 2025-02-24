using Ardalis.Result;
using DailyWire.Api.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using MediatR;

namespace DailyWire.Api.Queries;

public class GetPodcastSeasonsQuery : IRequest<Result<IList<DwSeasonDetails>>>
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
}

public class GetPodcastSeasonsResponse
{
    public DwGetPodcastRes GetPodcast { get; set; } = default!;
}

public class GetPodcastSeasonsQueryHandler(
    IGraphQLClient client
) : BaseDailyWireApiQueryHandler<GetPodcastSeasonsQuery, GetPodcastSeasonsResponse, IList<DwSeasonDetails>>(client)
{
    protected override GraphQLRequest BuildRequest(GetPodcastSeasonsQuery request) => new()
    {
        Query = @"
query getPodcastSeasons($id: ID, $slug: String) {
  getPodcast(where: { id: $id, slug: $slug }) {
    seasons {
        id
        name
        slug
        description
        __typename
    }
    __typename
  }
}
",
        Variables = request
    };

    protected override IList<DwSeasonDetails>? ExtractResponse(GetPodcastSeasonsResponse? response) => response?.GetPodcast.Seasons;
}