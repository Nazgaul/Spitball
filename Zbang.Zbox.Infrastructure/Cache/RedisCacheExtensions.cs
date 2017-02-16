using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Zbang.Zbox.Infrastructure.Cache
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Redis")]
    public static class RedisCacheExtensions
    {
        public static async Task<T> GetAsync<T>(this IDatabase cache, string key) where T : class
        {
            return Deserialize<T>(await cache.StringGetAsync(key));
        }

        public static T Get<T>(this IDatabase cache, string key) where T : class
        {
            return Deserialize<T>(cache.StringGet(key));
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

        private static T Deserialize<T>(string s) where T : class
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
