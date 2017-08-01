using System;
using System.Threading.Tasks;
using Microsoft.Spatial;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Zbang.Zbox.Infrastructure.Cache
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Redis")]
    public static class RedisCacheExtensions
    {
        public static async Task<T> GetAsync<T>(this IDatabase cache, string key) where T : class
        {
            return DeSerialize<T>(await cache.StringGetAsync(key).ConfigureAwait(false));
        }

        public static T Get<T>(this IDatabase cache, string key) where T : class
        {
            return DeSerialize<T>(cache.StringGet(key));
        }

        public static Task<bool> SetAsync<T>(this IDatabaseAsync cache, string key, T value, TimeSpan? expiry = null) where T : class
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            return cache.StringSetAsync(key, Serialize(value), expiry, flags: CommandFlags.FireAndForget);
        }

        public static bool Set<T>(this IDatabase cache, string key, T value, TimeSpan? expiry = null) where T : class
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            return cache.StringSet(key, Serialize(value), expiry, flags: CommandFlags.FireAndForget);
        }

        private static string Serialize<T>(T o) where T : class
        {
            if (o == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(o);
        }

        private static T DeSerialize<T>(string s) where T : class
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(s, new GeographyPointConverter());
        }
    }

    public class GeographyPointConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            //{"Latitude":42.0870521,"Longitude":-71.4061876,"IsEmpty":false,"Z":null,"M":null,"CoordinateSystem":{"EpsgId":4326,"Id":"4326","Name":"WGS84"}}
            var lat = token["Latitude"].ToObject<double>();
            var log = token["Longitude"].ToObject<double>();
            return GeographyPoint.Create(lat, log);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GeographyPoint);
        }
    }
}
