using Ardalis.Result;
using DailyWire.Api.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using MediatR;

namespace DailyWire.Api.Queries;

[Obsolete]
public class GetPodcastQuery : IRequest<Result<DwGetPodcastRes>>
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
}

[Obsolete]
public class GetPodcastQueryResponse
{
    public DwGetPodcastRes? GetPodcast { get; set; }
}

[Obsolete]
public class GetPodcastQueryHandler(IGraphQLClient client) : BaseDailyWireApiQueryHandler<GetPodcastQuery, GetPodcastQueryResponse, DwGetPodcastRes>(client)
{
  protected override GraphQLRequest BuildRequest(GetPodcastQuery request) => new()
    {
        Query = @"
fragment podcastFragment on getPodcastRes {
  id
  name
  slug
  status
  coverImage
  backgroundImage
  logoImage
  description
  belongsTo
  seasons {
    id
    name
    slug
    description
    __typename
  }
  author {
    id
    firstName
    lastName
    __typename
  }
  __typename
}

query getPodcast($id: ID, $slug: String) {
  getPodcast(where: { id: $id, slug: $slug }) {
    ...podcastFragment
    __typename
  }
}
",
        Variables = request
    };

    protected override DwGetPodcastRes? ExtractResponse(GetPodcastQueryResponse? response) => response?.GetPodcast;
}