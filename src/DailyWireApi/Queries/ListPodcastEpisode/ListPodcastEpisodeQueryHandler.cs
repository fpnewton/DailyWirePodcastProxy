using DailyWireApi.Models;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace DailyWireApi.Queries.ListPodcastEpisode;

public class ListPodcastEpisodeQueryHandler : BaseDailyWireApiQueryHandler<ListPodcastEpisodeQuery, ListPodcastEpisodeQueryResponse, IList<GetPodcastEpisodeRes>>
{
    public ListPodcastEpisodeQueryHandler(IGraphQLClient client) : base(client)
    {
    }

    protected override GraphQLRequest BuildRequest(ListPodcastEpisodeQuery request) => new()
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

fragment podcastEpisodeFragment on getPodcastEpisodeRes {
  id
  title
  description
  slug
  audio
  season {
    id
    name
    slug
    __typename
  }
  listenTime
  allowedContinents
  thumbnail
  duration
  rating
  status
  audioState
  podcast {
    ...podcastFragment
    __typename
  }
  publishDate
  createdAt
  updatedAt
  scheduleAt
  __typename
}

query listPodcastEpisode($seasonId: ID, $seasonSlug: String, $skip: Int, $first: Int) {
  listPodcastEpisode(where: { season: { id: $seasonId, slug: $seasonSlug } }, skip: $skip, first: $first) {
    ...podcastEpisodeFragment
    __typename
  }
}
",
        Variables = request
    };

    protected override IList<GetPodcastEpisodeRes>? ExtractResponse(ListPodcastEpisodeQueryResponse? response) => response?.ListPodcastEpisode;
}