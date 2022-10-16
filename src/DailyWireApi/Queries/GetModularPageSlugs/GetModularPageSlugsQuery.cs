using DailyWireApi.Models;
using MediatR;

namespace DailyWireApi.Queries.GetModularPageSlugs;

public class GetModularPageSlugsQuery : IRequest<IList<ModulePage>>
{
    public int? First { get; set; }
    public int? Skip { get; set; }
}