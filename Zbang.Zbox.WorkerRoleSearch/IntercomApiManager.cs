using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class IntercomApiManager : IIntercomApiManager
    {
        public async Task<IEnumerable<IntercomUsers>> GetUnsubscribersAsync(int page, CancellationToken token)
        {
            if (page < 1)
            {
                throw new ArgumentException("page need to be 1 or more");
            }
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                        "bmptcGdheXY6cm8tOTkyYzFiZTA4NzEwYTZjMWJkZGMyOGFmMWM0ZWQ3NTEzODQxMWY1YQ==");
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                using (
                    var response = await client.GetAsync($"https://api.intercom.io/users?segment_id=573177a948717f93ab00017a&page={page}", token))
                {
                    if (!response.IsSuccessStatusCode) return null;
                    using (var s = await response.Content.ReadAsStreamAsync())
                    {
                        using (var sr = new StreamReader(s))
                        {
                            using (var reader = new JsonTextReader(sr))
                            {
                                var obj = JObject.Load(reader);
                                var users = obj["users"];
                                return users.ToObject<IEnumerable<IntercomUsers>>();
                            }
                        }
                    }
                }
            }
        }
    }

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
                throw new JsonReaderException($"Unexcepted token {reader.TokenType}");
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(long);
            }
        }
    }



    public interface IIntercomApiManager
    {
        Task<IEnumerable<IntercomUsers>> GetUnsubscribersAsync(int page, CancellationToken token);
    }
}
