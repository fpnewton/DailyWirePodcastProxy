using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.Components;
using DailyWire.Api.Middleware.Models.Items;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;

namespace PodcastProxy.Application.Queries.Shows;

public class GetLatestShowEpisodesQuery : ICommand<Result<List<DwShowEpisode>>>
{
    public required string ShowSlug { get; set; }
}

public class GetLatestShowEpisodesQueryHandler(
    IDailyWireMiddlewareApi dwApiService
) : ICommandHandler<GetLatestShowEpisodesQuery, Result<List<DwShowEpisode>>>
{
    public async Task<Result<List<DwShowEpisode>>> ExecuteAsync(GetLatestShowEpisodesQuery command, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
            return userInfo.Map();

        var page = await dwApiService.GetShowPage(command.ShowSlug, userInfo.Value.AccessLevel, ct);

        if (!page.IsSuccess)
            return page.Map();

        var episodesTab = page.Value.Tabs.FirstOrDefault(tab => string.Equals(tab.Title, "Episodes", StringComparison.OrdinalIgnoreCase));

        if (episodesTab is null)
            return Result.NotFound();

        var showEpisodes = MapTabShowEpisodes(episodesTab).ToList();

        return Result.Success(showEpisodes);
    }

    private static IEnumerable<DwShowEpisode> MapTabShowEpisodes(DwTab tab)
    {
        foreach (var component in tab.Components)
        {
            if (component is not DwVerticalShowEpisodesCarouselComponent carousel)
                continue;
            
            foreach (var item in carousel.Items)
            {
                if (item is DwShowEpisodeItem episode)
                {
                    yield return episode.ShowEpisode;
                }
            }
        }
    }
}