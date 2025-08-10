using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastByIdQuery : ICommand<Result<Podcast>>
{
    public required string PodcastId { get; set; }
}

public class GetPodcastByIdQueryHandler(
    IRepository<Podcast> repository
) : ICommandHandler<GetPodcastByIdQuery, Result<Podcast>>
{
    public async Task<Result<Podcast>> ExecuteAsync(GetPodcastByIdQuery command, CancellationToken ct)
    {
        var podcastSpec = new PodcastByIdSpec(command.PodcastId);
        var podcast = await repository.FirstOrDefaultAsync(podcastSpec, ct);

        return podcast is not null ? Result.Success(podcast) : Result.NotFound();
    }
}