using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Commands.Podcasts;

public class EnsurePodcastExistsCommand : ICommand<Result>
{
    public required string PodcastId { get; set; }
}

public class EnsurePodcastExistsCommandHandler(
    IRepository<Podcast> repository
) : ICommandHandler<EnsurePodcastExistsCommand, Result>
{
    public async Task<Result> ExecuteAsync(EnsurePodcastExistsCommand command, CancellationToken ct)
    {
        var spec = new PodcastByIdSpec(command.PodcastId);
        var exists = await repository.AnyAsync(spec, ct);

        if (exists)
            return Result.Success();

        var podcast = await new FetchPodcastCommand { PodcastId = command.PodcastId }.ExecuteAsync(ct);
        
        await new CheckPodcastForNewEpisodesCommand { PodcastId = command.PodcastId }.ExecuteAsync(ct);

        return podcast.Map();
    }
}