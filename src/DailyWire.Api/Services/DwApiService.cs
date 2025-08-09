using Ardalis.Result;
using DailyWire.Api.Models;
using DailyWire.Api.Queries;
using MediatR;

namespace DailyWire.Api.Services;

[Obsolete]
public interface IDwApiService
{
    public Task<Result<DwModularPageRes>> GetModularPage(string slug, CancellationToken cancellationToken);

    public Task<Result<IList<DwModulePage>>> GetModularPageSlugs(CancellationToken cancellationToken);

    public Task<Result<DwGetPodcastRes>> GetPodcastById(string podcastId, CancellationToken cancellationToken);
    public Task<Result<DwGetPodcastRes>> GetPodcastBySlug(string podcastSlug, CancellationToken cancellationToken);

    public Task<Result<IList<DwSeasonDetails>>> GetPodcastSeasonsById(string podcastId, CancellationToken cancellationToken);

    public Task<Result<IList<DwSeasonDetails>>> GetPodcastSeasonsBySlug(string podcastSlug, CancellationToken cancellationToken);

    public Task<Result<DwGetPodcastEpisodeRes>> GetPodcastEpisodesBySlug(string slug, CancellationToken cancellationToken);

    public Task<Result<IList<DwGetPodcastEpisodeRes>>> GetPodcastEpisodesBySeason(string seasonId, int first, int skip, CancellationToken cancellationToken);
}

[Obsolete]
public class DwApiService(IMediator mediator) : IDwApiService
{
    public async Task<Result<DwModularPageRes>> GetModularPage(string slug, CancellationToken cancellationToken)
    {
        var query = new GetModularPageQuery
        {
            Slug = slug
        };

        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<IList<DwModulePage>>> GetModularPageSlugs(CancellationToken cancellationToken)
    {
        var query = new GetModularPageSlugsQuery();

        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<DwGetPodcastRes>> GetPodcastById(string podcastId, CancellationToken cancellationToken)
    {
        var query = new GetPodcastQuery { Id = podcastId };

        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<DwGetPodcastRes>> GetPodcastBySlug(string podcastSlug, CancellationToken cancellationToken)
    {
        var query = new GetPodcastQuery { Slug = podcastSlug };

        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<IList<DwSeasonDetails>>> GetPodcastSeasonsById(string podcastId, CancellationToken cancellationToken)
    {
        var query = new GetPodcastSeasonsQuery { Id = podcastId };

        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<IList<DwSeasonDetails>>> GetPodcastSeasonsBySlug(string podcastSlug, CancellationToken cancellationToken)
    {
        var query = new GetPodcastSeasonsQuery { Slug = podcastSlug };

        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<DwGetPodcastEpisodeRes>> GetPodcastEpisodesBySlug(string slug, CancellationToken cancellationToken)
    {
        var query = new GetPodcastEpisodeQuery { Slug = slug };

        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<IList<DwGetPodcastEpisodeRes>>> GetPodcastEpisodesBySeason(string seasonId, int first, int skip,
        CancellationToken cancellationToken)
    {
        var query = new ListPodcastEpisodeQuery
        {
            SeasonId = seasonId,
            First = first,
            Skip = skip
        };

        return await mediator.Send(query, cancellationToken);
    }
}