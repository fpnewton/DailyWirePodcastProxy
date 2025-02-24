using DailyWire.Api.Models;
using DailyWire.Api.Queries;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetModularPageBySlugRequest
{
    public string Slug { get; set; } = string.Empty;
}

public class GetModularPageBySlugEndpoint(IMediator mediator) : Endpoint<GetModularPageBySlugRequest>
{
    public override void Configure()
    {
#if DEBUG
        AllowAnonymous();
#endif
        
        Get("/daily-wire/modular-pages/{Slug}");
        Description(d => d.Produces<DwModularPageRes>());
    }

    public override async Task HandleAsync(GetModularPageBySlugRequest req, CancellationToken ct)
    {
        var query = new GetModularPageQuery
        {
            Slug = req.Slug
        };
        
        var result = await mediator.Send(query, ct);

        await this.SendResult(result, ct);
    }
}