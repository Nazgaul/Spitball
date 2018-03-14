using System;
using Cloudents.Core;
using Newtonsoft.Json;

namespace Cloudents.Api
{
    public class PrioritySourceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var p = (PrioritySource) value;
            writer.WriteValue(p.ToString());

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PrioritySource);
            
        }
    }
}