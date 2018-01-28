using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cloudents.Web.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, value);
                tempData.Add(key, Convert.ToBase64String(ms.ToArray()));
            }
            //tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            var o = tempData.Peek(key);
            if (o == null)
            {
                return null;
            }

            var bytes = Convert.FromBase64String((string)o);
            using (var ms = new MemoryStream(bytes))
            {
                var result = ProtoBuf.Serializer.Deserialize<T>(ms);
                return result;
            }
        }
    }
}
