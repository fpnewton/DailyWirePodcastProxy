using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Converters;

namespace DailyWire.Api.Middleware.Enums;

[JsonConverter(typeof(DwComponentRenderTypeConverter))]
public enum DwComponentRenderType
{
    Banner,
    ClipsCarousel,
    ClipsCarouselWhite,
    ContinueWatchingCarousel,
    FeaturedVerticalCarousel,
    FollowedHorizontalShowEpisodesCarousel,
    Generic,
    HomeFeaturedCarousel,
    HorizontalShowEpisodesCarousel,
    HostsBigCarousel,
    PortraitShowCarousel,
    PortraitShowsGrid,
    ShowsExclusiveCarousel,
    SquareShowCarousel,
    TopStory,
    VerticalCarousel,
    VerticalShowEpisodesCarousel
}