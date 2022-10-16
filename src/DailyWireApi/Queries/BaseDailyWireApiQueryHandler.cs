using DailyWireApi.Exceptions;
using DailyWireApi.Extensions;
using GraphQL;
using GraphQL.Client.Abstractions;
using MediatR;

namespace DailyWireApi.Queries;

public abstract class BaseDailyWireApiQueryHandler<TRequest, TResponseModel, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IGraphQLClient _client;

    protected BaseDailyWireApiQueryHandler(IGraphQLClient client)
    {
        _client = client;
    }

    public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var query = BuildRequest(request);
        var response = await _client.SendQueryAsync<TResponseModel>(query, cancellationToken);

        return ExtractResponse(response.Data) ?? throw new DailyWireApiException(response.ErrorMessage() ?? "Invalid response from DailyWire API");
    }

    protected abstract GraphQLRequest BuildRequest(TRequest request);

    protected abstract TResponse? ExtractResponse(TResponseModel? response);
}