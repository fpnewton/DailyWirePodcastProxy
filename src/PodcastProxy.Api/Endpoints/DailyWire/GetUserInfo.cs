using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetUserInfoEndpoint(IDailyWireMiddlewareApi api) : EndpointWithoutRequest<DwUserInfo>
{
    public override void Configure()
    {
        Get("/daily-wire/user-info");
        Description(d => d.Produces<DwUserInfo>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await api.GetUserInfo(ct);

        await this.SendResult(result, ct);
    }
}