using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastsQuery : ICommand<Result<List<Podcast>>>;

public class GetPodcastsQueryHandler(IRepository<Podcast> repository) : ICommandHandler<GetPodcastsQuery, Result<List<Podcast>>>
{
    public async Task<Result<List<Podcast>>> ExecuteAsync(GetPodcastsQuery command, CancellationToken ct)
    {
        var podcastSpec = new PodcastSpec();
        var podcasts = await repository.ListAsync(podcastSpec, ct);

        if (podcasts.Count < 1)
            return Result.NotFound();

        return Result.Success(podcasts);
    }
}