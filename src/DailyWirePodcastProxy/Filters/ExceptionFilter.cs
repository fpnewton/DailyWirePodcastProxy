using DailyWireApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DailyWirePodcastProxy.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case DailyWireApiException e:
                context.ExceptionHandled = true;

                context.Result = new ObjectResult(e.Message)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                break;
        }
    }
}