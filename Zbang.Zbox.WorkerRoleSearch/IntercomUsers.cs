using System;
using Newtonsoft.Json;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class IntercomUsers
    {
        [JsonProperty(PropertyName = "user_id", ItemConverterType = typeof(StringToLongConverter))]
        public long UserId { get; set; }
        public string Email { get; set; }

        private class StringToLongConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {

                if (reader.TokenType == JsonToken.Null)
                    throw new JsonReaderException($"Expected integer, got {reader.Value}");
                if (reader.TokenType == JsonToken.Integer)
                    return reader.Value;
                if (reader.TokenType == JsonToken.String)
                {
                    if (string.IsNullOrEmpty((string)reader.Value))
                        throw new JsonReaderException($"Expected integer, got {reader.Value}");
                    long num;
                    if (long.TryParse((string)reader.Value, out num))
                        return num;

                    throw new JsonReaderException($"Expected integer, got {reader.Value}");
                }
                throw new JsonReaderException($"UnExcepted token {reader.TokenType}");
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(long);
            }
        }
    }

    
}