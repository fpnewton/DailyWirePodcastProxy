using FastEndpoints;
using QRCoder;

namespace PodcastProxy.Api.Endpoints.QRCode;

public class GenerateQrCodeRequest
{
    public Uri Uri { get; set; } = null!;
}

public class GenerateQrCodeEndpoint : Endpoint<GenerateQrCodeRequest>
{
    public override void Configure()
    {
        Get("/qr-code");
    }

    public override async Task HandleAsync(GenerateQrCodeRequest req, CancellationToken ct)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(req.Uri.ToString(), QRCodeGenerator.ECCLevel.L);
        using var png = new PngByteQRCode(data);
        
        await Send.BytesAsync(png.GetGraphic(10), contentType: "image/png", cancellation: ct);
    }
}