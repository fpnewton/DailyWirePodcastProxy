using Ardalis.Result;
using FastEndpoints;
using PodcastProxy.Application.Queries.Shows;
using PodcastProxy.Database.Repositories;
using PodcastProxy.Domain.Specifications;
using Podcast = PodcastProxy.Domain.Entities.Podcast;

namespace PodcastProxy.Application.Commands.Podcasts;

public class FetchPodcastCommand : ICommand<Result<Podcast>>
{
    public string PodcastId { get; set; } = string.Empty;
}

public class FetchPodcastCommandHandler(IRepository<Podcast> repository) : ICommandHandler<FetchPodcastCommand, Result<Podcast>>
{
    public async Task<Result<Podcast>> ExecuteAsync(FetchPodcastCommand command, CancellationToken ct)
    {
        var result = await FetchPodcast(command, ct);

        if (!result.IsSuccess)
            return result;

        var spec = new PodcastByIdSpec(result.Value);
        var podcast = await repository.FirstOrDefaultAsync(spec, ct);

        if (podcast is null)
        {
            await repository.AddAsync(result.Value, ct);
        }
        else
        {
            podcast.Name = result.Value.Name;
            podcast.Description = result.Value.Description;
            podcast.Status = result.Value.Status;
            podcast.CoverImage = result.Value.CoverImage;
            podcast.BackgroundImage = result.Value.BackgroundImage;
            podcast.LogoImage = result.Value.LogoImage;
            podcast.BelongsTo = result.Value.BelongsTo;

            await repository.UpdateAsync(podcast, ct);
        }

        return result;
    }

    private static async Task<Result<Podcast>> FetchPodcast(FetchPodcastCommand command, CancellationToken ct)
    {
        var showItem = await new GetShowByIdQuery { Id = command.PodcastId }.ExecuteAsync(ct);

        if (!showItem.IsSuccess)
            return showItem.Map();

        var show = showItem.Value.Show;

        var podcast = new Podcast
        {
            Id = show.Id,
            Slug = show.Slug,
            Name = show.Title,
            Description = show.Description,
            CoverImage = new Uri(show.Images.Thumbnail.Landscape),
            BackgroundImage = new Uri(show.BackgroundImage),
            LogoImage = !string.IsNullOrEmpty(show.LogoImage) ? new Uri(show.LogoImage) : null
        };

        return Result.Success(podcast);
    }
}