using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TheRuleOfSilvester.Core.Options
{
    internal class OptionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Option);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var jObject = JObject.Load(reader);
                var name = jObject.GetValue("$type").ToString();
                jObject.Remove("$type");
                return new Option(jObject.ToObject(Type.GetType(name)));
            }
            else
            {
                return new Option(serializer.Deserialize(reader, objectType));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Option option))
                return;

            if (option.Value.GetType().IsPrimitive || option.Value is string)
            {
                serializer.Serialize(writer, option.Value);
            }
            else
            {
                var jObject = JObject.FromObject(option.Value);
                var type = option.Value.GetType();

                var name = type.IsNested ? type.AssemblyQualifiedName : type.FullName;

                jObject.Add("$type", JToken.FromObject(name));
                jObject.WriteTo(writer);
            }
        }
    }
}