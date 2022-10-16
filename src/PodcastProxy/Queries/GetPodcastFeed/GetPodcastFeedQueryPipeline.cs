using System.Xml.Linq;
using MediatR;
using PodcastDatabase.Contexts;
using PodcastDatabase.Entities;
using PodcastProxy.Commands.FetchLatestEpisodes;
using PodcastProxy.Commands.FetchPodcast;
using PodcastProxy.Exceptions;

namespace PodcastProxy.Queries.GetPodcastFeed;

public class GetPodcastFeedQueryPipeline : IPipelineBehavior<GetPodcastFeedQuery, XDocument>
{
    private readonly IMediator _mediator;
    private readonly PodcastDbContext _db;

    public GetPodcastFeedQueryPipeline(IMediator mediator, PodcastDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    public async Task<XDocument> Handle(GetPodcastFeedQuery request, RequestHandlerDelegate<XDocument> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (NotFoundException)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
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
            
        return await _mediator.Send(command, cancellationToken);
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

        await _mediator.Send(command, cancellationToken);
    }
}