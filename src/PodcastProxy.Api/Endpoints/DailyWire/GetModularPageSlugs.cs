using DailyWire.Api.Models;
using DailyWire.Api.Queries;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetModularPageSlugsRequest
{
    public int? First { get; set; }
    public int? Skip { get; set; }
}

public class GetModularPageSlugsEndpoint(IMediator mediator) : Endpoint<GetModularPageSlugsRequest>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif
        
        Get("/daily-wire/modular-pages");
        Description(d => d.Produces<IList<DwModulePage>>());
    }

    public override async Task HandleAsync(GetModularPageSlugsRequest req, CancellationToken ct)
    {
        var query = new GetModularPageSlugsQuery
        {
            First = req.First ?? 10,
            Skip = req.Skip ?? 0
        };
        
        var result = await mediator.Send(query, ct);

        await this.SendResult(result, ct);
    }
}