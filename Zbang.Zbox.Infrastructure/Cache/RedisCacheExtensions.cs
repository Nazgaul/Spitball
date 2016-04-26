using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public static class RedisCacheExtensions
    {
        public static T Get<T>(this IDatabase cache, string key) where T : class
        {
            if (cache == null)
            {
                return default(T);
            }
            return Deserialize<T>(cache.StringGet(key));
        }

        public static async Task<T> GetAsync<T>(this IDatabase cache, string key) where T : class
        {
            return Deserialize<T>(await cache.StringGetAsync(key));
        }

        public static bool Set<T>(this IDatabase cache, string key, T value, TimeSpan? expiry = null) where T : class
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));

            return cache.StringSet(key, Serialize(value), expiry);
        }

        public static Task<bool> SetAsync<T>(this IDatabase cache, string key, T value, TimeSpan? expiry = null) where T : class
        {
            return cache.StringSetAsync(key, Serialize(value), expiry);
        }

        static string Serialize<T>(T o) where T : class
        {
            if (o == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(o);
        }

        static T Deserialize<T>(string s) where T : class
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
