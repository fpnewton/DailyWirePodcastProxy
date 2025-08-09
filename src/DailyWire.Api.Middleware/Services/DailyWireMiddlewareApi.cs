using System.Text.Json;
using Ardalis.Result;
using DailyWire.Api.Middleware.Configuration;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using DailyWire.Authentication.Services;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace DailyWire.Api.Middleware.Services;

public interface IDailyWireMiddlewareApi
{
    public Task<Result<DwUserInfo>> GetUserInfo(CancellationToken cancellationToken);
    public Task<Result<DwUserInfo>> GetUserInfo(bool noCache, CancellationToken cancellationToken);

    public Task<Result<DwPage>> GetPage(string slug, string membershipPlan, CancellationToken cancellationToken);

    public Task<Result<DwShowPage>> GetShowPage(string slug, string membershipPlan, CancellationToken cancellationToken);

    public Task<Result<DwPaginatedPage>> GetPaginatedEpisodes(string slug, string showSeasonId, string? lastPodcastEpisodeId, string? lastShowEpisodeId,
        int showOffset, int podcastOffset, int pageNumber, int pageSize, string orderBy, DwSortOrderDirection orderDirection, string membershipPlan,
        CancellationToken cancellationToken);

    public Task<Result<DwEpisodeDetails>> GetEpisode(string slug, CancellationToken cancellationToken);
}

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

    public async Task<Result<DwPaginatedPage>> GetPaginatedEpisodes(string slug, string showSeasonId, string? lastPodcastEpisodeId, string? lastShowEpisodeId,
        int showOffset, int podcastOffset, int pageNumber, int pageSize, string orderBy, DwSortOrderDirection orderDirection, string membershipPlan,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await tokenService.GetAccessToken(cancellationToken);

            var response = await configuration.BaseUrl
                .AppendPathSegments("v4", "getPaginatedEpisodes")
                .SetQueryParam("slug", slug)
                .SetQueryParam("showSeasonId", showSeasonId)
                .SetQueryParam("lastPodcastEpisodeId", lastPodcastEpisodeId)
                .SetQueryParam("lastShowEpisodeId", lastShowEpisodeId)
                .SetQueryParam("showOffset", showOffset)
                .SetQueryParam("podcastOffset", podcastOffset)
                .SetQueryParam("pageNumber", pageNumber)
                .SetQueryParam("pageSize", pageSize)
                .SetQueryParam("orderBy", $"{orderBy}_\"{JsonSerializer.Serialize(orderDirection)}\"")
                .SetQueryParam("membershipPlan", membershipPlan)
                .WithOAuthBearerToken(token)
                .GetJsonAsync<DwPaginatedPage>(cancellationToken: cancellationToken);

            return Result.Success(response);
        }
        catch (FlurlHttpException e)
        {
            var error = await e.GetResponseStringAsync();

            logger.LogError(e, "Failed to get paginated episodes '{Slug}/{ShowSeasonId}': {Message} = {Error}", slug, showSeasonId, e.Message, error);

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