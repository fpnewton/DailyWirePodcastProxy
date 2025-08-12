using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastBySlugQuery : ICommand<Result<Podcast>>
{
    public required string Slug { get; set; }
}

public class GetPodcastBySlugQueryHandler(
    IRepository<Podcast> repository
) : ICommandHandler<GetPodcastBySlugQuery, Result<Podcast>>
{
    public async Task<Result<Podcast>> ExecuteAsync(GetPodcastBySlugQuery command, CancellationToken ct)
    {
        var podcastSpec = new PodcastBySlugSpec(command.Slug);
        var podcast = await repository.FirstOrDefaultAsync(podcastSpec, ct);

        return podcast is not null ? Result.Success(podcast) : Result.NotFound();
    }
}