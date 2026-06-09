using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Tests.Entities;

public class SeasonTests
{
    [Fact]
    public void DefaultComparer_EqualSeasonsHaveEqualHashCodes()
    {
        var first = CreateSeason("season-1", "podcast-1", "season-slug");
        var second = CreateSeason("season-2", "PODCAST-1", "SEASON-SLUG");

        Assert.True(Season.DefaultComparer.Equals(first, second));
        Assert.Equal(
            Season.DefaultComparer.GetHashCode(first),
            Season.DefaultComparer.GetHashCode(second));
    }

    [Fact]
    public void DefaultComparer_ReconcilesMatchingSlugAndPodcastAsExisting()
    {
        var fetched = CreateSeason("new-season-id", "podcast-1", "season-slug");
        var stored = CreateSeason("stored-season-id", "PODCAST-1", "SEASON-SLUG");

        Assert.Empty(new[] { fetched }.Except(new[] { stored }, Season.DefaultComparer));
        Assert.Equal(fetched, new[] { fetched }.Intersect(new[] { stored }, Season.DefaultComparer).Single());
    }

    private static Season CreateSeason(string seasonId, string podcastId, string slug) => new()
    {
        SeasonId = seasonId,
        PodcastId = podcastId,
        Slug = slug
    };
}
