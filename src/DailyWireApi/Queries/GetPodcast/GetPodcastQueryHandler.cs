using DailyWireApi.Models;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace DailyWireApi.Queries.GetPodcast;

public class GetPodcastQueryHandler : BaseDailyWireApiQueryHandler<GetPodcastQuery, GetPodcastQueryResponse, GetPodcastRes>
{
    public GetPodcastQueryHandler(IGraphQLClient client) : base(client)
    {
    }

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

    protected override GetPodcastRes? ExtractResponse(GetPodcastQueryResponse? response) => response?.GetPodcast;
}