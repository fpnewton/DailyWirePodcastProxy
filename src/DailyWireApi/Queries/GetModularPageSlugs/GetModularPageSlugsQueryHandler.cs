using DailyWireApi.Models;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace DailyWireApi.Queries.GetModularPageSlugs;

public class GetModularPageSlugsQueryHandler : BaseDailyWireApiQueryHandler<GetModularPageSlugsQuery, GetModularPageSlugsQueryResponse, IList<ModulePage>>
{
    public GetModularPageSlugsQueryHandler(IGraphQLClient client) : base(client)
    {
    }

    public override async Task<IList<ModulePage>> Handle(GetModularPageSlugsQuery request, CancellationToken cancellationToken)
    {
        if (!request.First.HasValue)
        {
            var modulePages = new List<ModulePage>();
            IList<ModulePage> response;

            request.First = 10;
            request.Skip = 0;

            do
            {
                response = await base.Handle(request, cancellationToken);
                response = response.Except(modulePages, ModulePage.TypenameIdComparer).ToList();

                modulePages.AddRange(response);
                request.Skip += request.First;
            } while (response.Count > 0);

            return modulePages;
        }
        else
        {
            return await base.Handle(request, cancellationToken);
        }
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

    protected override IList<ModulePage>? ExtractResponse(GetModularPageSlugsQueryResponse? response) => response?.ModulePages?.OrderBy(page => page.Slug).ToList();
}