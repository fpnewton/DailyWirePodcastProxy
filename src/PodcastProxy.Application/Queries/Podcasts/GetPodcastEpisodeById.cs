using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries.Podcasts;

public class GetPodcastEpisodeByIdQuery : ICommand<Result<Episode>>
{
    public required string EpisodeId { get; set; }
}

public class GetPodcastEpisodeByIdQueryHandler(
    IRepository<Episode> repository
    ) : ICommandHandler<GetPodcastEpisodeByIdQuery, Result<Episode>>
{
    public async Task<Result<Episode>> ExecuteAsync(GetPodcastEpisodeByIdQuery command, CancellationToken ct)
    {
        var episodeSpec = new EpisodeByIdSpec(command.EpisodeId);
        var episode = await repository.FirstOrDefaultAsync(episodeSpec, ct);
        
        return episode is not null ? Result.Success(episode) : Result.NotFound();
    }
}