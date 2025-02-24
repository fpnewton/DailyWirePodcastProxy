using Ardalis.Result;
using GraphQL;
using GraphQL.Client.Abstractions;
using MediatR;

namespace DailyWire.Api.Queries;

public abstract class BaseDailyWireApiQueryHandler<TRequest, TResponseModel, TResponse>(IGraphQLClient client) : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    public virtual async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var query = BuildRequest(request);
        var response = await client.SendQueryAsync<TResponseModel>(query, cancellationToken);

        if (response.Errors is not null && response.Errors.Any())
        {
            var errors = new ErrorList(response.Errors.Select(e => e.Message));

            return Result.Error(errors);
        }
        
        var data = ExtractResponse(response.Data);

        if (data is null)
        {
            return Result.Error("Invalid response from DailyWire API");
        }

        return Result.Success(data);
    }

    protected abstract GraphQLRequest BuildRequest(TRequest request);

    protected abstract TResponse? ExtractResponse(TResponseModel? response);
}