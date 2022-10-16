using DailyWireApi.Models;
using MediatR;

namespace DailyWireApi.Queries.GetModularPage;

public class GetModularPageQuery : IRequest<ModularPageRes>
{
    public string Slug { get; set; } = string.Empty;
}