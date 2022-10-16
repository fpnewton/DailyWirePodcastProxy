using DailyWireApi.Models;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace DailyWireApi.Queries.GetModularPage;

public class GetModularPageQueryHandler : BaseDailyWireApiQueryHandler<GetModularPageQuery, GetModularPageQueryResponse, ModularPageRes>
{
    public GetModularPageQueryHandler(IGraphQLClient client) : base(client)
    {
    }

    protected override GraphQLRequest BuildRequest(GetModularPageQuery request) => new()
    {
        Query = @"
fragment modularPodcastFragment on Podcast {
  id
  name
  slug
  description
  belongsTo
  logoImage
  coverImage
  backgroundImage
  author {
    id
    firstName
    lastName
    __typename
  }
  __typename
}

query getModularPage($slug: String!) {
  getModularPage(where: { slug: $slug }) {
    id
    slug
    nextOrder
    modules {
      __typename
      ... on ClipCarousel {
        id
        type
        title
        layout
        clips {
          id
          title: name
          image
          video {
            id
            slug
            __typename
          }
          __typename
        }
        __typename
      }
      ... on VideoCarousel {
        id
        type
        title
        layout
        videos {
          id
          description
          title: name
          image
          logoImage
          __typename
        }
        __typename
      }
      ... on ShowCarousel {
        id
        type
        orientation
        title
        shows {
          id
          name
          slug
          thumbnail
          image
          belongsTo
          portraitImage
          description
          __typename
        }
        __typename
      }
      ... on EpisodeCarousel {
        id
        type
        order
        title
        episodes {
          id
          image
          title
          status
          description
          createdAt
          show {
            id
            name
            __typename
          }
          __typename
        }
        __typename
      }
      ... on ShowPremiere {
        id
        type
        premiereImage
        show {
          id
          name
          slug
          backgroundImage
          logoImage
          portraitImage
          thumbnail
          belongsTo
          description
          __typename
        }
        __typename
      }
      ... on EpisodePremiere {
        id
        type
        premiereImage
        episode {
          id
          title
          createdAt
          description
          status
          image
          show {
            id
            name
            slug
            __typename
          }
          __typename
        }
        __typename
      }
      ... on VideoPremiere {
        id
        type
        premiereImage
        video {
          id
          name
          image
          logoImage
          description
          status
          __typename
        }
        __typename
      }
      ... on Headliner {
        id
        type
        title
        description
        referenceId
        referenceType
        referenceSlug
        routing
        CTAText
        link
        item {
          __typename
          ... on Clip {
            video {
              id
              slug
              __typename
            }
            __typename
          }
          ... on Show {
            belongsTo
            portraitImage
            __typename
          }
        }
        __typename
      }
      ... on Highlight {
        id
        image
        title
        type
        description
        referenceId
        referenceType
        referenceSlug
        routing
        CTAText
        link
        item {
          __typename
          ... on Clip {
            video {
              id
              slug
              __typename
            }
            __typename
          }
          ... on Show {
            belongsTo
            portraitImage
            __typename
          }
        }
        __typename
      }
      ... on ContinueWatching {
        id
        type
        title
        __typename
      }
      ... on PodcastPremiere {
        id
        type
        premiereImage
        podcast {
          ...modularPodcastFragment
          __typename
        }
        __typename
      }
      ... on ContinueListening {
        id
        type
        title
        __typename
      }
      ... on PodcastEpisodePremiere {
        id
        type
        premiereImage
        podcastEpisode {
          id
          title
          description
          slug
          createdAt
          thumbnail
          publishDate
          podcast {
            ...modularPodcastFragment
            __typename
          }
          __typename
        }
        __typename
      }
      ... on PodcastCarousel {
        id
        type
        title
        podcasts {
          ...modularPodcastFragment
          __typename
        }
        __typename
      }
      ... on PodcastEpisodeCarousel {
        id
        title
        type
        podcastEpisodes {
          id
          title
          slug
          thumbnail
          status
          duration
          audio
          createdAt
          publishDate
          podcast {
            ...modularPodcastFragment
            __typename
          }
          __typename
        }
        __typename
      }
    }
    __typename
  }
}
",
        Variables = request
    };

    protected override ModularPageRes? ExtractResponse(GetModularPageQueryResponse? response) => response?.GetModularPage;
}