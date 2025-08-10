using Ardalis.Result;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;

namespace PodcastProxy.Application.Queries.Shows;

public class GetShowSeasonsByShowIdQuery : ICommand<Result<List<DwEntity>>>
{
    public required string ShowId { get; set; }
}

public class GetShowSeasonsByShowIdQueryHandler(
    IDailyWireMiddlewareApi dwApiService
) : ICommandHandler<GetShowSeasonsByShowIdQuery, Result<List<DwEntity>>>
{
    public async Task<Result<List<DwEntity>>> ExecuteAsync(GetShowSeasonsByShowIdQuery command, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
            return userInfo.Map();

        var show = await new GetShowByIdQuery { Id = command.ShowId }.ExecuteAsync(ct);

        if (!show.IsSuccess)
            return show.Map();

        var page = await dwApiService.GetShowPage(show.Value.Show.Slug, userInfo.Value.AccessLevel, ct);

        return page.Map(p => p.Show.Seasons);
    }
}