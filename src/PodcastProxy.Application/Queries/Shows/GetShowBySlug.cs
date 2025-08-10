using Ardalis.Result;
using DailyWire.Api.Middleware.Models.Items;
using FastEndpoints;

namespace PodcastProxy.Application.Queries.Shows;

public class GetShowBySlugQuery : ICommand<Result<DwShowItem>>
{
    public required string Slug { get; set; }
}

public class GetShowBySlugQueryHandler : ICommandHandler<GetShowBySlugQuery,  Result<DwShowItem>>
{
    public async Task<Result<DwShowItem>> ExecuteAsync(GetShowBySlugQuery command, CancellationToken ct)
    {
        var shows = await new GetShowsQuery().ExecuteAsync(ct);

        if (!shows.IsSuccess)
            return shows.Map();
        
        var show = shows.Value.FirstOrDefault(s => string.Equals(s.Show.Slug, command.Slug, StringComparison.OrdinalIgnoreCase));

        if (show is null)
            return Result.NotFound();

        return Result.Success(show);
    }
}