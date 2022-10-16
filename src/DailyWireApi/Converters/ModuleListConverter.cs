using DailyWireApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DailyWireApi.Converters;

public class ModuleListConverter : JsonConverter
{
    private IEnumerable<Type> ModuleTypes => GetType()
        .Assembly
        .GetTypes()
        .Where(t => t.IsAssignableTo(typeof(IModule)))
        .Where(t => t.IsClass && !t.IsAbstract);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is IList<IModule> modules)
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

        return ParseJsonArray(array, serializer).ToList();
    }

    private IEnumerable<IModule> ParseJsonArray(JArray array, JsonSerializer serializer)
    {
        foreach (var token in array)
        {
            var typeProps = new ModuleType();
            
            serializer.Populate(token.CreateReader(), typeProps);

            var moduleType = ModuleTypes.SingleOrDefault(t => string.Equals(t.Name, typeProps.Typename));

            if (moduleType is null)
                continue;

            var module = (IModule?) token.ToObject(moduleType, serializer);

            if (module is not null)
                yield return module;
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(IModule));
    }
    
    private class ModuleType : IModule
    {
        public string Typename { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Type { get; set; } = null!;

        public override string ToString() => Typename;
    }
}