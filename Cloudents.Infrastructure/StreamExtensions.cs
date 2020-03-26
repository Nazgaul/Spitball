using System.IO;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure
{
    public static class StreamExtensions
    {
        //public static T ToJsonReader<T>(this Stream s, Func<JsonTextReader, T> func)
        //{

        //    using (var sr = new StreamReader(s))
        //    using (var reader = new JsonTextReader(sr))
        //    {
        //        return func(reader);
        //    }
        //}

        public static T ToJsonReader<T>(this Stream s, JsonSerializer? serializer = null)
        {
            using var sr = new StreamReader(s);
            using var reader = new JsonTextReader(sr);
            if (serializer == null)
            {
                serializer = new JsonSerializer();
            }

            return serializer.Deserialize<T>(reader);
        }
    }


}