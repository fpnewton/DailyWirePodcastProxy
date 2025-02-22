using System.Xml.Linq;
using MediatR;
using PodcastProxy.Application.Commands;
using PodcastProxy.Application.Commands.FetchLatestEpisodes;
using PodcastProxy.Application.Exceptions;
using PodcastProxy.Database.Contexts;
using PodcastProxy.Domain.Entities;

namespace PodcastProxy.Application.Queries.GetPodcastFeed;

public class GetPodcastFeedQueryPipeline(IMediator mediator, PodcastDbContext db) : IPipelineBehavior<GetPodcastFeedQuery, XDocument>
{
    public async Task<XDocument> Handle(GetPodcastFeedQuery request, RequestHandlerDelegate<XDocument> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (NotFoundException)
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var podcast = await FetchPodcast(request, cancellationToken);

            await FetchPodcastEpisodes(podcast, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await next();
        }
    }

    private async Task<Podcast> FetchPodcast(GetPodcastFeedQuery request, CancellationToken cancellationToken)
    {
        var command = new FetchPodcastCommand
        {
            PodcastId = request.PodcastId
        };

        return await mediator.Send(command, cancellationToken);
    }

    private async Task FetchPodcastEpisodes(Podcast podcast, CancellationToken cancellationToken)
    {
        var season = podcast.Seasons.MaxBy(s => s.Slug);

        if (season is null)
        {
            throw new NotFoundException($"Latest {nameof(Season)} for podcast", podcast.Id);
        }

        var command = new FetchLatestEpisodesCommand
        {
            SeasonId = season.SeasonId
        };

        await mediator.Send(command, cancellationToken);
    }
}