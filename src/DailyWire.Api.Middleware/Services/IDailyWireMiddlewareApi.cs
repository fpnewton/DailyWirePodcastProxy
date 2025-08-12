using Ardalis.Result;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;

namespace DailyWire.Api.Middleware.Services;

public interface IDailyWireMiddlewareApi
{
    public Task<Result<DwUserInfo>> GetUserInfo(CancellationToken cancellationToken);
    public Task<Result<DwUserInfo>> GetUserInfo(bool noCache, CancellationToken cancellationToken);

    public Task<Result<DwPage>> GetPage(string slug, string membershipPlan, CancellationToken cancellationToken);

    public Task<Result<DwShowPage>> GetShowPage(string slug, string membershipPlan, CancellationToken cancellationToken);

    public Task<Result<DwEpisodeDetails>> GetEpisode(string slug, CancellationToken cancellationToken);
}