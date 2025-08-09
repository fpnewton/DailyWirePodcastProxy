using Ardalis.Result;
using DailyWire.Api.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using MediatR;

namespace DailyWire.Api.Queries;

[Obsolete]
public class GetModularPageSlugsQuery : IRequest<Result<IList<DwModulePage>>>
{
    public int First { get; set; }
    public int Skip { get; set; }
}

[Obsolete]
public class GetModularPageSlugsQueryResponse
{
    public IList<DwModulePage>? ModulePages { get; set; }
}

[Obsolete]
public class GetModularPageSlugsQueryHandler(
    IGraphQLClient client
) : BaseDailyWireApiQueryHandler<GetModularPageSlugsQuery, GetModularPageSlugsQueryResponse, IList<DwModulePage>>(client)
{
    public override async Task<Result<IList<DwModulePage>>> Handle(GetModularPageSlugsQuery request, CancellationToken cancellationToken)
    {
        if (request.First < 1)
        {
            request.First = 10;
        }

        var modulePages = new List<DwModulePage>();

        request.First = 10;
        request.Skip = 0;

        do
        {
            var result = await base.Handle(request, cancellationToken);

            if (!result.IsSuccess)
            {
                return Result.Error(new ErrorList(result.Errors));
            }
            
            var data = result.Value.Except(modulePages, DwModulePage.TypenameIdComparer).ToList();

            if (data.Count < 1)
            {
                break;
            }

            modulePages.AddRange(data);
            request.Skip += request.First;
        } while (true);

        return modulePages;
    }

    protected override GraphQLRequest BuildRequest(GetModularPageSlugsQuery request) => new()
    {
        Query = @"
query getModularPageSlugs($first: Int!, $skip: Int) {
    modulePages(where: {isPublished: true}, first: $first, skip: $skip) {
        __typename
        id
        slug
    }
}
",
        Variables = request
    };

    protected override IList<DwModulePage>? ExtractResponse(GetModularPageSlugsQueryResponse? response) =>
        response?.ModulePages?.OrderBy(page => page.Slug).ToList();
}