using Ardalis.Result;
using DailyWire.Api.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using MediatR;

namespace DailyWire.Api.Queries;

public class ListPodcastEpisodeQuery : IRequest<Result<IList<DwGetPodcastEpisodeRes>>>
{
    public string? SeasonId { get; set; }
    public int? First { get; set; }
    public int? Skip { get; set; }
}

public class ListPodcastEpisodeQueryResponse
{
    public IList<DwGetPodcastEpisodeRes>? ListPodcastEpisode { get; set; }
}

public class ListPodcastEpisodeQueryHandler(
    IGraphQLClient client
) : BaseDailyWireApiQueryHandler<ListPodcastEpisodeQuery, ListPodcastEpisodeQueryResponse, IList<DwGetPodcastEpisodeRes>>(client)
{
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

    protected override IList<DwGetPodcastEpisodeRes>? ExtractResponse(ListPodcastEpisodeQueryResponse? response) => response?.ListPodcastEpisode;
}