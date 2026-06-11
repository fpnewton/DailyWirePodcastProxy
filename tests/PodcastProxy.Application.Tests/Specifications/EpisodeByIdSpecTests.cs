using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PodcastProxy.Database.Contexts;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Tests.Specifications;

public class EpisodeByIdSpecTests
{
    [Fact]
    public async Task QueryReturnsTrackedInstanceWhenEpisodeIsAlreadyTracked()
    {
        var options = new DbContextOptionsBuilder<PodcastDbContext>()
            .UseInMemoryDatabase($"{nameof(EpisodeByIdSpecTests)}-{Guid.NewGuid():N}")
            .Options;
        await using var context = new PodcastDbContext(options);
        var trackedEpisode = new Episode
        {
            EpisodeId = "episode-1",
            SeasonId = "season-1",
            Slug = "episode-1"
        };

        context.Episodes.Add(trackedEpisode);
        await context.SaveChangesAsync();

        var queriedEpisode = await context.Episodes
            .WithSpecification(new EpisodeByIdSpec(trackedEpisode.EpisodeId))
            .SingleAsync();

        Assert.Same(trackedEpisode, queriedEpisode);
    }
}
