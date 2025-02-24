using DailyWire.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DailyWire.Api.Converters;

public class ItemConverter : JsonConverter
{
    private IEnumerable<Type> ItemTypes => GetType()
        .Assembly
        .GetTypes()
        .Where(t => t.IsAssignableTo(typeof(IDwItem)))
        .Where(t => t.IsClass && !t.IsAbstract);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is IDwItem item)
        {
            serializer.Serialize(writer, value);
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var token = JObject.Load(reader);
        var typeProps = new ItemType();

        serializer.Populate(token.CreateReader(), typeProps);

        var itemType = ItemTypes.SingleOrDefault(t => string.Equals(t.Name, typeProps.Typename));

        if (itemType is not null)
        {
            var item = (IDwItem?)token.ToObject(itemType, serializer);

            if (item is not null)
                return item;
        }

        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(IDwItem));
    }

    private class ItemType : IDwItem
    {
        public string Typename { get; set; } = null!;
        public string Id { get; set; } = null!;
    }
}