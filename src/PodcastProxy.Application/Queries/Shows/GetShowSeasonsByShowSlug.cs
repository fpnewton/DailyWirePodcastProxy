using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;

namespace PodcastProxy.Application.Queries.Shows;

public class GetShowSeasonsByShowSlugQuery : ICommand<Result<List<DwEntity>>>
{
    public required string Slug { get; set; }
}

public class GetShowSeasonsByShowSlugHandler(
    IDailyWireMiddlewareApi dwApiService
    ) : ICommandHandler<GetShowSeasonsByShowSlugQuery,  Result<List<DwEntity>>>
{
    public async Task<Result<List<DwEntity>>> ExecuteAsync(GetShowSeasonsByShowSlugQuery command, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
            return userInfo.Map();

        var show = await new GetShowBySlugQuery { Slug = command.Slug }.ExecuteAsync(ct);

        if (!show.IsSuccess)
            return show.Map();

        var page = await dwApiService.GetShowPage(show.Value.Show.Slug, userInfo.Value.AccessLevel, ct);

        return page.Map(p => p.Show.Seasons);
    }
}