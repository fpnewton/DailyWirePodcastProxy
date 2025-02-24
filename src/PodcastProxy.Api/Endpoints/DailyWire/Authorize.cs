using DailyWire.Authentication.Handlers;
using DailyWire.Authentication.TokenStorage;
using FastEndpoints;

namespace PodcastProxy.Api.Endpoints.DailyWire;

public class AuthorizeRequest
{
    public string Code { get; set; } = string.Empty;
}

public class AuthorizeEndpoint(ITokenStore tokenStore, DeviceCodeLoginHandler handler) : Endpoint<AuthorizeRequest>
{
    public override void Configure()
    {
        Post("/authorize");
    }

    public override async Task HandleAsync(AuthorizeRequest req, CancellationToken ct)
    {
        var result = await handler.GetUserToken(req.Code, ct);

        if (!result.IsSuccess)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        await tokenStore.StoreAuthenticationTokensAsync(result.Value, ct);
        
        await SendOkAsync(ct);
    }
}