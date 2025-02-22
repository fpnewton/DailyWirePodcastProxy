using Newtonsoft.Json;

namespace DailyWire.Api.Models;

public class DwModulePage
{
    [JsonProperty("__typename")]
    public string Typename { get; set; } = null!;
    
    public string Id { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Title { get; set; }

    private sealed class TypenameIdEqualityComparer : IEqualityComparer<DwModulePage>
    {
        public bool Equals(DwModulePage? x, DwModulePage? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.Typename == y.Typename && x.Id == y.Id;
        }

        public int GetHashCode(DwModulePage obj)
        {
            return HashCode.Combine(obj.Typename, obj.Id);
        }
    }

    public static IEqualityComparer<DwModulePage> TypenameIdComparer { get; } = new TypenameIdEqualityComparer();
}