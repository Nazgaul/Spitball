using System;
using Cloudents.Core;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure
{
    public class EnumerationConverter<T> : JsonConverter<T?> where T : Enumeration
    {
        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }
            writer.WriteValue(value.Name);
        }

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var val = reader.Value;
            if (val == null)
            {
                return null;
            }
            return Enumeration.FromDisplayName<T>(val.ToString());

        }
    }
}