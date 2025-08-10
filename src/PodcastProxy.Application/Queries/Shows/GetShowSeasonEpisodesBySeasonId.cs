using Ardalis.Result;
using DailyWire.Api.Middleware.Enums;
using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Models.ComponentItems;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;
using Flurl;
using Microsoft.Extensions.Configuration;

namespace PodcastProxy.Application.Queries.Shows;

public class GetShowSeasonEpisodesBySeasonIdQuery : ICommand<Result<PaginatedGetShowSeasonEpisodesBySeasonId>>
{
    public required string ShowSlug { get; set; }
    public required string SeasonId { get; set; }
    public string? LastPodcastEpisodeId { get; set; }
    public string? LastShowEpisodeId { get; set; }
    public int? ShowOffset { get; set; }
    public int? PodcastOffset { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? OrderBy { get; set; }
    public DwSortOrderDirection? OrderDirection { get; set; }
}

public class GetShowSeasonEpisodesBySeasonIdQueryHandler(
    IConfiguration configuration,
    IDailyWireMiddlewareApi dwApiService
) : ICommandHandler<GetShowSeasonEpisodesBySeasonIdQuery, Result<PaginatedGetShowSeasonEpisodesBySeasonId>>
{
    public async Task<Result<PaginatedGetShowSeasonEpisodesBySeasonId>> ExecuteAsync(GetShowSeasonEpisodesBySeasonIdQuery command, CancellationToken ct)
    {
        var userInfo = await dwApiService.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
            return userInfo.Map();

        var page = await dwApiService.GetPaginatedEpisodes(
            command.ShowSlug, command.SeasonId, command.LastPodcastEpisodeId, command.LastShowEpisodeId,
            command.ShowOffset ?? 0, command.PodcastOffset ?? 0,
            command.PageNumber ?? 1, command.PageSize ?? 10,
            command.OrderBy ?? "CreatedAt",
            command.OrderDirection ?? DwSortOrderDirection.Descending,
            userInfo.Value.AccessLevel, ct
        );

        if (!page.IsSuccess)
            return page.Map();

        GetShowSeasonEpisodesBySeasonIdQuery? nextPage = null;

        if (!string.IsNullOrEmpty(page.Value.NextPageUrl))
        {
            var nextPageUrl = new Url(page.Value.NextPageUrl);

            T? GetNextPageUrlQueryParam<T>(string queryParamName)
            {
                if (!nextPageUrl.QueryParams.Contains(queryParamName))
                    return default;

                var value = nextPageUrl.QueryParams.FirstOrDefault(queryParamName);

                return (T)Convert.ChangeType(value, typeof(T));
            }

            nextPage = new GetShowSeasonEpisodesBySeasonIdQuery
            {
                ShowSlug = command.ShowSlug,
                SeasonId = command.SeasonId,
                LastPodcastEpisodeId = GetNextPageUrlQueryParam<string>("lastPodcastEpisodeId"),
                LastShowEpisodeId = GetNextPageUrlQueryParam<string>("lastShowEpisodeId"),
                ShowOffset = GetNextPageUrlQueryParam<int>("showOffset"),
                PodcastOffset = GetNextPageUrlQueryParam<int>("podcastOffset"),
                PageNumber = GetNextPageUrlQueryParam<int>("pageNumber"),
                PageSize = GetNextPageUrlQueryParam<int>("pageSize"),
                OrderBy = GetNextPageUrlQueryParam<string>("orderBy"),
                OrderDirection = Enum.TryParse<DwSortOrderDirection>(GetNextPageUrlQueryParam<string>("orderDirection"), true, out var sortDir) ? sortDir : null
            };
        }

        return page.Map(p => new PaginatedGetShowSeasonEpisodesBySeasonId
        {
            Episodes = GetPageShowEpisodes(p).ToList(),
            NextPage = nextPage
        });
    }

    private static IEnumerable<DwShowEpisode> GetPageShowEpisodes(DwPaginatedPage page)
    {
        foreach (var component in page.ComponentItems ?? [])
        {
            if (component is not DwShowEpisodeComponentItem showEpisode)
            {
                continue;
            }

            yield return showEpisode.ShowEpisode;
        }
    }
}

public class PaginatedGetShowSeasonEpisodesBySeasonId
{
    public IList<DwShowEpisode> Episodes { get; set; } = [];
    public GetShowSeasonEpisodesBySeasonIdQuery? NextPage { get; set; }
}