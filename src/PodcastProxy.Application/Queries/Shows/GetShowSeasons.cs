using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;

namespace PodcastProxy.Application.Queries.Shows;

public class GetShowSeasonsQuery : ICommand<Result<List<DwEntity>>>
{
    public required string ShowSlug { get; set; }
}

public class GetShowSeasonsQueryHandler(
    IDailyWireMiddlewareApi dwApiService
) : ICommandHandler<GetShowSeasonsQuery, Result<List<DwEntity>>>
{
    public async Task<Result<List<DwEntity>>> ExecuteAsync(GetShowSeasonsQuery command, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
            return userInfo.Map();

        var page = await dwApiService.GetShowPage(command.ShowSlug, userInfo.Value.AccessLevel, ct);

        return page.Map(p => p.Show.Seasons);
    }
}