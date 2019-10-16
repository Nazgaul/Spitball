using System;
using System.Collections.Generic;
using System.IO;
using Cloudents.Core.Interfaces;
using Newtonsoft.Json;
using ProtoBuf;

namespace Cloudents.Infrastructure
{
    public class BinarySerializer : IBinarySerializer
    {
        private static void Serialize(Stream sr, object o)
        {
            Serializer.Serialize(sr, o);

        }

        private static T Deserialize<T>(Stream sr)
        {
            return Serializer.Deserialize<T>(sr);
        }

        public byte[] Serialize(object o)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, o);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] sr)
        {
            using (var ms = new MemoryStream(sr))
            {
                return Deserialize<T>(sr: ms);
            }
        }
    }

    public class SbJsonSerializer : IJsonSerializer
    {
        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public T Deserialize<T>(string sr)
        {
            return JsonConvert.DeserializeObject<T>(sr);
        }

        public T Deserialize<T>(string sr, JsonConverterTypes types)
        {
            var jsonConverterList = new List<JsonConverter>();

            foreach (Enum value in Enum.GetValues(typeof(JsonConverterTypes)))
                if (types.HasFlag(value))
                {
                    var type = (JsonConverterTypes) value;
                    switch (type)
                    {
                        case JsonConverterTypes.None:
                            break;
                        case JsonConverterTypes.TimeSpan:
                            jsonConverterList.Add(new TimespanConverter());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

            return JsonConvert.DeserializeObject<T>(sr, jsonConverterList.ToArray());
        }
    }



    public class TimespanConverter : JsonConverter<TimeSpan?>
    {
       
        public override void WriteJson(JsonWriter writer, TimeSpan? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override TimeSpan? ReadJson(JsonReader reader, Type objectType, TimeSpan? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value is null)
            {
                return null;
            }
            var ticks = (long) reader.Value;
            return new TimeSpan(ticks);
        }
    }
}