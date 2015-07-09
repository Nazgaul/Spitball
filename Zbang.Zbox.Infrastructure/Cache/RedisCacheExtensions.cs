﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        static string Serialize<T>(T o) where T : class
        {
            if (o == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(o);
            //var pformatter = new Transport.ProtobufSerializer<T>();
            //return pformatter.SerializeData(o);

            //var binaryFormatter = new BinaryFormatter();
            //using (var memoryStream = new MemoryStream())
            //{
            //    binaryFormatter.Serialize(memoryStream, o);
            //    var objectDataAsStream = memoryStream.ToArray();
            //    return objectDataAsStream;
            //}
        }

        static T Deserialize<T>(string s) where T : class
        {
            if (string.IsNullOrEmpty(s))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(s);
            //var pformatter = new Transport.ProtobufSerializer<T>();
            //return pformatter.DeSerializeData(stream);
            //var binaryFormatter = new BinaryFormatter();
            //using (var memoryStream = new MemoryStream(stream))
            //{
            //    var result = (T)binaryFormatter.Deserialize(memoryStream);
            //    return result;
            //}
        }
    }
}
