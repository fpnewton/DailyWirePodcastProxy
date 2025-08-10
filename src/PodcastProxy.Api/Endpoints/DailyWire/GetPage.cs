using DailyWire.Api.Middleware.Models;
using DailyWire.Api.Middleware.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using PodcastProxy.Api.Extensions;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class GetPageRequest
{
    public required string Slug { get; set; }
}

public class GetPageEndpoint(IDailyWireMiddlewareApi api) : Endpoint<GetPageRequest, DwPage>
{
    public override void Configure()
    {
        Get("/daily-wire/pages/{Slug}");
        Description(d => d.Produces<DwPage>());
    }

    public override async Task HandleAsync(GetPageRequest req, CancellationToken ct)
    {
        var userInfo = await api.GetUserInfo(ct);

        if (!userInfo.IsSuccess)
        {
            AddError("Failed to get user info.");

            await Send.ErrorsAsync(StatusCodes.Status412PreconditionFailed, ct);
            return;
        }

        var result = await api.GetPage(req.Slug, userInfo.Value.AccessLevel, ct);
        
        await this.SendResult(result, ct);
    }
}