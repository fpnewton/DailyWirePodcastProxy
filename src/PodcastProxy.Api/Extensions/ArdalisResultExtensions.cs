using Ardalis.Result;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using IResult = Ardalis.Result.IResult;

namespace PodcastProxy.Api.Extensions;

public static class ArdalisResultExtensions
{
    public static async Task SendResult<TResult>(this IEndpoint ep, TResult result, CancellationToken ct)
        where TResult : IResult
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
                await ep.HttpContext.Response.SendAsync(result.GetValue(), cancellation: ct);
                break;

            case ResultStatus.Created:
                break;

            case ResultStatus.Error:
                foreach (var error in result.Errors)
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        ep.ValidationFailures.Add(new ValidationFailure("", error));
                    }
                }

                await ep.HttpContext.Response.SendErrorsAsync(ep.ValidationFailures, StatusCodes.Status500InternalServerError, cancellation: ct);
                break;

            case ResultStatus.Forbidden:
                await ep.HttpContext.Response.SendForbiddenAsync(ct);
                break;

            case ResultStatus.Unauthorized:
                await ep.HttpContext.Response.SendUnauthorizedAsync(ct);
                break;

            case ResultStatus.Invalid:
                foreach (var error in result.ValidationErrors)
                {
                        ep.ValidationFailures.Add(new ValidationFailure(error.Identifier, error.ErrorMessage));
                }

                await ep.HttpContext.Response.SendErrorsAsync(ep.ValidationFailures, cancellation: ct);
                break;

            case ResultStatus.NotFound:
                await ep.HttpContext.Response.SendNotFoundAsync(ct);
                break;

            case ResultStatus.NoContent:
                await ep.HttpContext.Response.SendNoContentAsync(ct);
                break;

            case ResultStatus.Conflict:
                foreach (var error in result.Errors)
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        ep.ValidationFailures.Add(new ValidationFailure("", error));
                    }
                }

                await ep.HttpContext.Response.SendErrorsAsync(ep.ValidationFailures, StatusCodes.Status409Conflict, cancellation: ct);
                break;

            case ResultStatus.CriticalError:
                foreach (var error in result.Errors)
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        ep.ValidationFailures.Add(new ValidationFailure("", error));
                    }
                }

                await ep.HttpContext.Response.SendErrorsAsync(ep.ValidationFailures, StatusCodes.Status500InternalServerError, cancellation: ct);
                break;

            case ResultStatus.Unavailable:
                foreach (var error in result.Errors)
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        ep.ValidationFailures.Add(new ValidationFailure("", error));
                    }
                }

                await ep.HttpContext.Response.SendErrorsAsync(ep.ValidationFailures, StatusCodes.Status503ServiceUnavailable, cancellation: ct);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}