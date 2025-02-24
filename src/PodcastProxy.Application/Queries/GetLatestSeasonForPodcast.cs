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
        var existingSeasons = await repository.ListAsync(spec, cancellationToken);
        var newSeasons = await GetLatestSeasonsApi(request.PodcastId, cancellationToken);

        foreach (var newSeason in newSeasons)
        {
            if (!existingSeasons.Any(s => string.Equals(s.SeasonId, newSeason.SeasonId, StringComparison.Ordinal)))
            {
                await repository.AddAsync(newSeason, cancellationToken);
            }
        }

        return Result.Success(newSeasons.First());
    }

    private async Task<IList<Season>> GetLatestSeasonsApi(string podcastId, CancellationToken cancellationToken)
    {
        var command = new FetchPodcastCommand
        {
            PodcastId = podcastId
        };

        var result = await mediator.Send(command, cancellationToken);
        var podcast = result.Value;

        return podcast.Seasons.ToList();
    }
}