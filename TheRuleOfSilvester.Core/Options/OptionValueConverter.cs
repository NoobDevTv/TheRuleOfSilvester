using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TheRuleOfSilvester.Core.Options
{
    internal class OptionValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {

                var jObject = JObject.Load(reader);
                var name = jObject.GetValue("$type").ToString();
                jObject.Remove("$type");
                return jObject.ToObject(Type.GetType(name));
            }
            else
            {
                return serializer.Deserialize(reader, objectType);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType().IsPrimitive || value is string)
            {
                serializer.Serialize(writer, value);
            }
            else
            {
                var jObject = JObject.FromObject(value);
                var type = value.GetType();

                var name = type.IsNested ? type.AssemblyQualifiedName : type.FullName;

                jObject.Add("$type", JToken.FromObject(name));
                jObject.WriteTo(writer);
            }
        }
    }
}