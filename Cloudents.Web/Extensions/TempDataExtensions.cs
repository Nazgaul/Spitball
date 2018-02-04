using System;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cloudents.Web.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value, IStreamSerializer serializer) where T : class
        {
            var bytes = serializer.Serialize(value);
            tempData[key] = Convert.ToBase64String(bytes);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key, IStreamSerializer serializer) where T : class
        {
            var o = tempData.Peek(key);
            if (o == null)
            {
                return null;
            }

            var bytes = Convert.FromBase64String((string)o);
            return serializer.DeSerialize<T>(bytes);
        }
    }
}
