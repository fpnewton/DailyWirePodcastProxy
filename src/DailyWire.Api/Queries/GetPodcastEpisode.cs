using Ardalis.Result;
using DailyWire.Api.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using MediatR;

namespace DailyWire.Api.Queries;

public class GetPodcastEpisodeQuery : IRequest<Result<DwGetPodcastEpisodeRes>>
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
}

public class GetPodcastEpisodeQueryResponse
{
    public DwGetPodcastEpisodeRes? GetPodcastEpisode { get; set; }
}

public class GetPodcastEpisodeQueryHandler(
    IGraphQLClient client
) : BaseDailyWireApiQueryHandler<GetPodcastEpisodeQuery, GetPodcastEpisodeQueryResponse, DwGetPodcastEpisodeRes>(client)
{
    protected override GraphQLRequest BuildRequest(GetPodcastEpisodeQuery request) => new()
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

query getPodcastEpisode($id: ID, $slug: String) {
  getPodcastEpisode(where: { id: $id, slug: $slug }) {
    ...podcastEpisodeFragment
    __typename
  }
}
",
        Variables = request
    };

    protected override DwGetPodcastEpisodeRes? ExtractResponse(GetPodcastEpisodeQueryResponse? response) => response?.GetPodcastEpisode;
}