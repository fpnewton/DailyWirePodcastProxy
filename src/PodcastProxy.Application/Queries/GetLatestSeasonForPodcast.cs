using Ardalis.Result;
using MediatR;
using PodcastProxy.Application.Commands;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Queries;

public class GetLatestSeasonForPodcastQuery : IRequest<Result<Season>>
{
    public string PodcastId { get; set; } = string.Empty;
}

public class GetLatestSeasonForPodcastQueryHandler(
    IRepository<Season> repository,
    IMediator mediator
) : IRequestHandler<GetLatestSeasonForPodcastQuery, Result<Season>>
{
    public async Task<Result<Season>> Handle(GetLatestSeasonForPodcastQuery request, CancellationToken cancellationToken)
    {
        var spec = new SeasonByPodcastIdSpec(request.PodcastId);
        var season = await repository.FirstOrDefaultAsync(spec, cancellationToken);
        var latest = await GetLatestSeasonApi(request.PodcastId, cancellationToken);

        if (latest is not null)
        {
            if (season is null || !string.Equals(season.SeasonId, latest.SeasonId, StringComparison.Ordinal))
            {
                await repository.AddAsync(latest, cancellationToken);

                season = latest;
            }
        }

        return season is not null ? Result<Season>.Success(season) : Result<Season>.NotFound();
    }

    private async Task<Season?> GetLatestSeasonApi(string podcastId, CancellationToken cancellationToken)
    {
        var command = new FetchPodcastCommand
        {
            PodcastId = podcastId
        };

        var result = await mediator.Send(command, cancellationToken);
        var podcast = result.Value;

        return podcast.Seasons.MaxBy(s => s.Slug);
    }
}