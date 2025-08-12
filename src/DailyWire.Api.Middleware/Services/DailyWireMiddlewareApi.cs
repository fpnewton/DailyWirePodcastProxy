using Ardalis.Result;
using DailyWire.Api.Middleware.Configuration;
using DailyWire.Api.Middleware.Models;
using DailyWire.Authentication.Services;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace DailyWire.Api.Middleware.Services;

public class DailyWireMiddlewareApi(
    ILogger<DailyWireMiddlewareApi> logger,
    ITokenService tokenService,
    DwMiddlewareConfiguration configuration
) : IDailyWireMiddlewareApi
{
    public Task<Result<DwUserInfo>> GetUserInfo(CancellationToken cancellationToken) =>
        GetUserInfo(true, cancellationToken);

    public async Task<Result<DwUserInfo>> GetUserInfo(bool noCache, CancellationToken cancellationToken)
    {
        try
        {
            var token = await tokenService.GetAccessToken(cancellationToken);

            var response = await configuration.BaseUrl
                .AppendPathSegments("v3", "getUserInfo")
                .SetQueryParam("nocache", noCache ? 1 : 0)
                .WithOAuthBearerToken(token)
                .GetJsonAsync<DwUserInfo>(cancellationToken: cancellationToken);

            return Result.Success(response);
        }
        catch (FlurlHttpException e)
        {
            var error = await e.GetResponseStringAsync();

            logger.LogError(e, "Failed to get user info: {Message} = {Error}", e.Message, error);

            return Result.Error(error);
        }
    }

    public async Task<Result<DwPage>> GetPage(string slug, string membershipPlan, CancellationToken cancellationToken)
    {
        try
        {
            var token = await tokenService.GetAccessToken(cancellationToken);

            var response = await configuration.BaseUrl
                .AppendPathSegments("v4", "getPage")
                .SetQueryParam("slug", slug)
                .SetQueryParam("membershipPlan", membershipPlan)
                .WithOAuthBearerToken(token)
                .GetJsonAsync<DwPage>(cancellationToken: cancellationToken);

            return Result.Success(response);
        }
        catch (FlurlHttpException e)
        {
            var error = await e.GetResponseStringAsync();

            logger.LogError(e, "Failed to get page '{Slug}': {Message} = {Error}", slug, e.Message, error);

            return Result.Error(error);
        }
    }

    public async Task<Result<DwShowPage>> GetShowPage(string slug, string membershipPlan, CancellationToken cancellationToken)
    {
        try
        {
            var token = await tokenService.GetAccessToken(cancellationToken);

            var response = await configuration.BaseUrl
                .AppendPathSegments("v4", "getShowPage")
                .SetQueryParam("slug", slug)
                .SetQueryParam("membershipPlan", membershipPlan)
                .WithOAuthBearerToken(token)
                .GetJsonAsync<DwShowPage>(cancellationToken: cancellationToken);

            return Result.Success(response);
        }
        catch (FlurlHttpException e)
        {
            var error = await e.GetResponseStringAsync();

            logger.LogError(e, "Failed to get show page '{Slug}': {Message} = {Error}", slug, e.Message, error);

            return Result.Error(error);
        }
    }

    public async Task<Result<DwEpisodeDetails>> GetEpisode(string slug, CancellationToken cancellationToken)
    {
        try
        {
            var token = await tokenService.GetAccessToken(cancellationToken);

            var response = await configuration.BaseUrl
                .AppendPathSegments("v4", "getEpisode")
                .SetQueryParam("slug", slug)
                .WithOAuthBearerToken(token)
                .GetJsonAsync<DwEpisodeDetails>(cancellationToken: cancellationToken);

            return Result.Success(response);
        }
        catch (FlurlHttpException e)
        {
            var error = await e.GetResponseStringAsync();

            logger.LogError(e, "Failed to get episode '{Slug}': {Message} = {Error}", slug, e.Message, error);

            return Result.Error(error);
        }
    }
}