using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.Components;
using DailyWire.Api.Middleware.Models.Items;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;
using PodcastProxy.Domain.Models;

namespace PodcastProxy.Application.Queries.Shows;

public class GetShowsQuery : ICommand<Result<List<DwShowItem>>>;

public class GetShowsQueryHandler(
    DailyWireConfig config,
    IDailyWireMiddlewareApi dwApiService
) : ICommandHandler<GetShowsQuery, Result<List<DwShowItem>>>
{
    public async Task<Result<List<DwShowItem>>> ExecuteAsync(GetShowsQuery command, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
        {
            return userInfo.Map();
        }

        var page = await dwApiService.GetPage(config.ShowPageSlug, userInfo.Value.AccessLevel, ct);

        return page.Map(p => GetPageShows(p).ToList());
    }

    private static IEnumerable<DwShowItem> GetPageShows(DwPage page)
    {
        foreach (var component in page.Components)
        {
            if (component is not DwSquareShowCarouselComponent showCarousel)
            {
                continue;
            }

            foreach (var item in showCarousel.Items)
            {
                if (item is not DwShowItem show)
                {
                    continue;
                }

                yield return show;
            }
        }
    }
}