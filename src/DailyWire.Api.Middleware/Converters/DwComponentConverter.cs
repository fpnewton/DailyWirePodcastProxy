using System.Text.Json;
using System.Text.Json.Serialization;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.Components;

namespace DailyWire.Api.Middleware.Converters;

public class DwComponentConverter : JsonConverter<DwComponent>
{
    private static readonly Dictionary<DwComponentRenderType, Type> TypeMap = new()
    {
        { DwComponentRenderType.Banner, typeof(DwBannerComponent) },
        { DwComponentRenderType.ClipsCarousel, typeof(DwClipsCarouselComponent) },
        { DwComponentRenderType.ClipsCarouselWhite, typeof(DwClipsCarouselWhiteComponent) },
        { DwComponentRenderType.ContinueWatchingCarousel, typeof(DwContinueWatchingCarouselComponent) },
        { DwComponentRenderType.FeaturedVerticalCarousel, typeof(DwFeaturedVerticalCarouselComponent) },
        { DwComponentRenderType.FollowedHorizontalShowEpisodesCarousel, typeof(DwFollowedHorizontalShowEpisodesCarouselComponent) },
        { DwComponentRenderType.Generic, typeof(DwGenericComponent) },
        { DwComponentRenderType.HomeFeaturedCarousel, typeof(DwHomeFeaturedCarouselComponent) },
        { DwComponentRenderType.HorizontalShowEpisodesCarousel, typeof(DwHorizontalShowEpisodesCarouselComponent) },
        { DwComponentRenderType.HostsBigCarousel, typeof(DwHostsBigCarouselComponent) },
        { DwComponentRenderType.PortraitShowCarousel, typeof(DwPortraitShowCarouselComponent) },
        { DwComponentRenderType.PortraitShowsGrid, typeof(DwPortraitShowsGridComponent) },
        { DwComponentRenderType.ShowsExclusiveCarousel, typeof(DwShowsExclusiveCarouselComponent) },
        { DwComponentRenderType.SquareShowCarousel, typeof(DwSquareShowCarouselComponent) },
        { DwComponentRenderType.TopStory, typeof(DwTopStoryComponent) },
        { DwComponentRenderType.VerticalCarousel, typeof(DwVerticalCarouselComponent) },
        { DwComponentRenderType.VerticalShowEpisodesCarousel, typeof(DwVerticalShowEpisodesCarouselComponent) }
    };

    public override DwComponent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("renderType", out var typeProp))
        {
            throw new JsonException("Missing discriminator property 'renderType'.");
        }

        // Use our enum converter to parse renderType
        var renderType = JsonSerializer.Deserialize<DwComponentRenderType>(typeProp.GetRawText(), options);

        if (!TypeMap.TryGetValue(renderType, out var targetType))
        {
            Console.Error.WriteLine($"No mapping registered for DwComponentRenderType '{renderType}'.");
            return null;
        }

        return (DwComponent?)JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
    }

    public override void Write(Utf8JsonWriter writer, DwComponent value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}