using AutoMapper;
using DailyWireApi.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DailyWirePodcastProxy.Controllers;

public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;
    private IMapper? _mapper;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();

    protected async Task<IActionResult> InvokeMediatorRequest<TRequest>(object model, CancellationToken cancellationToken) where TRequest : IBaseRequest
    {
        try
        {
            var request = Mapper.Map<TRequest>(model);
            var result = await Mediator.Send(request, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (DailyWireApiException e)
        {
            return new ObjectResult(e.Message)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}