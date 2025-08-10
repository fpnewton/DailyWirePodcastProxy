using Ardalis.Result;
using DailyWire.Api.Middleware.Models.Items;
using FastEndpoints;

namespace PodcastProxy.Application.Queries.Shows;

public class GetShowByIdQuery : ICommand<Result<DwShowItem>>
{
    public required string Id { get; set; }
}

public class GetShowByIdQueryHandler : ICommandHandler<GetShowByIdQuery, Result<DwShowItem>>
{
    public async Task<Result<DwShowItem>> ExecuteAsync(GetShowByIdQuery command, CancellationToken ct)
    {
        var shows = await new GetShowsQuery().ExecuteAsync(ct);

        if (!shows.IsSuccess)
            return shows.Map();
        
        var show = shows.Value.FirstOrDefault(s => string.Equals(s.Show.Id, command.Id, StringComparison.Ordinal));

        if (show is null)
            return Result.NotFound();

        return Result.Success(show);
    }
}