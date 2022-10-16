using System.Text;
using GraphQL;

namespace DailyWireApi.Extensions;

public static class GraphQlResponseExtensions
{
    public static string? ErrorMessage<T>(this GraphQLResponse<T> response) =>
        response.Errors?.Aggregate(new StringBuilder(), (builder, error) => builder.AppendLine(error.Message)).ToString();
}