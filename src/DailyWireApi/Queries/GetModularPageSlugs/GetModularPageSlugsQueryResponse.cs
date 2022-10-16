using DailyWireApi.Models;

namespace DailyWireApi.Queries.GetModularPageSlugs;

public class GetModularPageSlugsQueryResponse
{
    public IList<ModulePage>? ModulePages { get; set; }
}