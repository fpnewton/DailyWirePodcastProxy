using System.Text.Json;
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

            var request = configuration.BaseUrl
                .AppendPathSegments("v3", "getUserInfo")
                .SetQueryParam("nocache", noCache ? 1 : 0)
                .WithOAuthBearerToken(token);

            var response = await GetJsonResponse<DwUserInfo>(request, "getUserInfo", cancellationToken);

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

            var request = configuration.BaseUrl
                .AppendPathSegments("v4", "getPage")
                .SetQueryParam("slug", slug)
                .SetQueryParam("membershipPlan", membershipPlan)
                .WithOAuthBearerToken(token);

            var response = await GetJsonResponse<DwPage>(request, $"getPage/{slug}", cancellationToken);

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

            var request = configuration.BaseUrl
                .AppendPathSegments("v4", "getShowPage")
                .SetQueryParam("slug", slug)
                .SetQueryParam("membershipPlan", membershipPlan)
                .WithOAuthBearerToken(token);

            var response = await GetJsonResponse<DwShowPage>(request, $"getShowPage/{slug}", cancellationToken);

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

            var request = configuration.BaseUrl
                .AppendPathSegments("v4", "getEpisode")
                .SetQueryParam("slug", slug)
                .WithOAuthBearerToken(token);

            var response = await GetJsonResponse<DwEpisodeDetails>(request, $"getEpisode/{slug}", cancellationToken);

            return Result.Success(response);
        }
        catch (FlurlHttpException e)
        {
            var error = await e.GetResponseStringAsync();

            logger.LogError(e, "Failed to get episode '{Slug}': {Message} = {Error}", slug, e.Message, error);

            return Result.Error(error);
        }
    }

    private async Task<T> GetJsonResponse<T>(
        IFlurlRequest request,
        string operation,
        CancellationToken cancellationToken)
    {
        var rawJson = await request.GetStringAsync(cancellationToken: cancellationToken);

        if (configuration.LogRawJsonResponses)
        {
            logger.LogInformation(
                "Raw DailyWire API JSON response for {Operation}: {RawJson}",
                operation,
                rawJson);
        }

        return request.Settings.JsonSerializer.Deserialize<T>(rawJson)
               ?? throw new JsonException($"DailyWire API returned a null response for '{operation}'.");
    }
}
