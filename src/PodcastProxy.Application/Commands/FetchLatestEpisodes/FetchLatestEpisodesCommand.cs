using AutoMapper;
using DailyWire.Api.Queries;
using MediatR;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Entities;
using PodcastProxy.Domain.Specifications;

namespace PodcastProxy.Application.Commands.FetchLatestEpisodes;

public class FetchLatestEpisodesCommand : IRequest<ICollection<Episode>>
{
    public string SeasonId { get; set; } = null!;
    public int First { get; set; } = 10;
}

public class FetchLatestEpisodesCommandHandler(IMapper mapper, IMediator mediator, IRepository<Episode> repository)
    : IRequestHandler<FetchLatestEpisodesCommand, ICollection<Episode>>
{
    public async Task<ICollection<Episode>> Handle(FetchLatestEpisodesCommand request, CancellationToken cancellationToken)
    {
        var query = new ListPodcastEpisodeQuery { SeasonId = request.SeasonId, First = request.First, Skip = 0 };
        var models = await mediator.Send(query, cancellationToken);
        var episodes = new List<Episode>(models.Value.Count);

        foreach (var model in models.Value)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                continue;
            }

            if (!string.Equals(model.Status, "PUBLISHED", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var spec = new EpisodeByIdSpec(model.Id);
            var exists = await repository.AnyAsync(spec, cancellationToken);
            var episode = mapper.Map<Episode>(model);

            episode.SeasonId = request.SeasonId;

            if (!exists)
            {
                episode = await repository.AddAsync(episode, cancellationToken);
            }
            else
            {
                await repository.UpdateAsync(episode, cancellationToken);
            }

            episodes.Add(episode);
        }

        return episodes;
    }
}