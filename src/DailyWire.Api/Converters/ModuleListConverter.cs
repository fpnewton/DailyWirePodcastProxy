using DailyWire.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DailyWire.Api.Converters;

[Obsolete]
public class ModuleListConverter : JsonConverter
{
    private IList<IDwModule> Modules => GetType()
        .Assembly
        .GetTypes()
        .Where(t => t.IsAssignableTo(typeof(IDwModule)))
        .Where(t => t is { IsClass: true, IsAbstract: false })
        .Select(Activator.CreateInstance)
        .Cast<IDwModule>()
        .ToList();

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is IList<IDwModule> modules)
        {
            writer.WriteStartArray();

            foreach (var module in modules)
            {
                serializer.Serialize(writer, module);
            }

            writer.WriteEndArray();
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var array = JArray.Load(reader);
        var modules = ParseJsonArray(array, serializer).ToList();

        return modules;
    }

    private IEnumerable<IDwModule> ParseJsonArray(JArray array, JsonSerializer serializer)
    {
        foreach (var token in array)
        {
            var module = ParseModule(token, serializer);

            if (module is not null)
                yield return module;
        }
    }

    private IDwModule? ParseModule(JToken token, JsonSerializer serializer)
    {
        var typeProps = new ModuleType();

        serializer.Populate(token.CreateReader(), typeProps);

        var moduleType = Modules
            .Where(t => string.Equals(t.Typename, typeProps.Typename, StringComparison.OrdinalIgnoreCase))
            .Select(t => t.GetType())
            .SingleOrDefault();

        if (moduleType is not null)
        {
            return (IDwModule?)token.ToObject(moduleType, serializer);
        }

        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(IDwModule));
    }

    [Obsolete]
    private class ModuleType : IDwModule
    {
        [JsonProperty("__typename")]
        public string Typename { get; set; } = null!;

        public string Id { get; set; } = null!;
        public string Type { get; set; } = null!;

        public override string ToString() => Typename;
    }
}