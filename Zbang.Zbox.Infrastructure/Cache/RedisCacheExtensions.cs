using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public static class RedisCacheExtensions
    {
        public static T Get<T>(this IDatabase cache, string key) where T : class
        {
            return Deserialize<T>(cache.StringGet(key));
        }

        public async static Task<T> GetAsync<T>(this IDatabase cache, string key) where T : class
        {
            return Deserialize<T>(await cache.StringGetAsync(key));
        }

        public static bool Set<T>(this IDatabase cache, string key, T value, TimeSpan? expiry = null) where T : class
        {
            return cache.StringSet(key, Serialize(value), expiry);
        }

        public static Task<bool> SetAsync<T>(this IDatabase cache, string key, T value, TimeSpan? expiry = null) where T : class
        {
            return cache.StringSetAsync(key, Serialize(value), expiry);
        }

        static byte[] Serialize<T>(T o) where T : class
        {
            if (o == null)
            {
                return null;
            }
            var pformatter = new Transport.ProtobufSerializer<T>();
            return pformatter.SerializeData(o);

            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    binaryFormatter.Serialize(memoryStream, o);
            //    byte[] objectDataAsStream = memoryStream.ToArray();
            //    return objectDataAsStream;
            //}
        }

        static T Deserialize<T>(byte[] stream) where T : class
        {
            if (stream == null)
            {
                return default(T);
            }
            var pformatter = new Transport.ProtobufSerializer<T>();
            return pformatter.DeSerializeData(stream);
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream(stream))
            //{
            //    T result = (T)binaryFormatter.Deserialize(memoryStream);
            //    return result;
            //}
        }
    }
}
